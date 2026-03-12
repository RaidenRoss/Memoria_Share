using Assets.Scripts.Common;
using Assets.SiliconSocial;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ShopUI
{
    private const String DevTitle = "DevMenu";
    private const String DevQtyHelp = "Quantità da aggiungere";
    private const String DevItemsHelp = "Scegli oggetto da aggiungere";
    private const String DevKeyItemsHelp = "Scegli rarità da aggiungere";
    private const String DevCardsHelp = "Scegli carta da aggiungere";
    private const String DevItemsTabLabel = "Oggetti";
    private const String DevKeyItemsTabLabel = "Rarità";
    private const String DevCardsTabLabel = "Carte";
    private const String DevItemsSubMenuHelp = "Per scegliere gli oggetti\nda aggiungere nell'inventario.";
    private const String DevKeyItemsSubMenuHelp = "Per scegliere le rarità\nda aggiungere nell'inventario.";
    private const String DevCardsSubMenuHelp = "Per scegliere le carte\nda aggiungere nella collezione.";
    private String originalBuySubMenuHelpText;
    private String originalBuySubMenuHelpTextKey;
    private String originalSellSubMenuHelpText;
    private String originalSellSubMenuHelpTextKey;
    private Boolean hasCachedOriginalSubMenuHelp;
    private String originalItemFundCaptionText;
    private String originalItemCountCaptionText;
    private Boolean hasCachedOriginalItemInfoCaptions;

    private Boolean isDevMode;

    private enum DevMenuCategory
    {
        Items,
        KeyItems,
        Cards
    }

    private DevMenuCategory currentDevCategory = DevMenuCategory.Items;
    private readonly List<Int32> devKeyItemIdList = new List<Int32>();
    private readonly List<TetraMasterCardId> devCardIdList = new List<TetraMasterCardId>();
    private GameObject devCardsSubMenu;
    private Vector3 originalBuySubMenuLocalPosition;
    private Vector3 originalSellSubMenuLocalPosition;
    private Vector3 originalBuySubMenuLocalScale;
    private Vector3 originalSellSubMenuLocalScale;
    private Boolean hasCachedOriginalDevTabLayout;
    private UILabel GetItemFundCaptionLabel()
    {
        return this.ItemInfoPanel.GetChild(0).GetChild(0).GetComponent<UILabel>();
    }

    private UILabel GetItemCountCaptionLabel()
    {
        return this.ItemInfoPanel.GetChild(1).GetChild(0).GetComponent<UILabel>();
    }

    private void CacheOriginalItemInfoCaptionsIfNeeded()
    {
        if (this.hasCachedOriginalItemInfoCaptions)
            return;

        this.originalItemFundCaptionText = this.GetItemFundCaptionLabel().rawText;
        this.originalItemCountCaptionText = this.GetItemCountCaptionLabel().rawText;
        this.hasCachedOriginalItemInfoCaptions = true;
    }

    private void RestoreOriginalItemInfoCaptions()
    {
        if (!this.hasCachedOriginalItemInfoCaptions)
            return;

        this.GetItemFundCaptionLabel().rawText = this.originalItemFundCaptionText;
        this.GetItemCountCaptionLabel().rawText = this.originalItemCountCaptionText;
    }
    private UILabel FindSubMenuLabel(GameObject subMenuRoot)
    {
        if (subMenuRoot == null)
            return null;

        Transform directLabel = subMenuRoot.transform.Find("Label");
        if (directLabel != null)
            return directLabel.GetComponent<UILabel>();

        Transform buyButtonLabel = subMenuRoot.transform.Find("Buy Button/Label");
        if (buyButtonLabel != null)
            return buyButtonLabel.GetComponent<UILabel>();

        Transform sellButtonLabel = subMenuRoot.transform.Find("Sell Button/Label");
        if (sellButtonLabel != null)
            return sellButtonLabel.GetComponent<UILabel>();

        foreach (Transform child in subMenuRoot.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "Label")
                return child.GetComponent<UILabel>();
        }

        return null;
    }
    private void CacheOriginalSubMenuHelpIfNeeded()
    {
        if (this.hasCachedOriginalSubMenuHelp)
            return;

        this.CacheSingleSubMenuHelp(
            this.BuySubMenu,
            out this.originalBuySubMenuHelpText,
            out this.originalBuySubMenuHelpTextKey);

        this.CacheSingleSubMenuHelp(
            this.SellSubMenu,
            out this.originalSellSubMenuHelpText,
            out this.originalSellSubMenuHelpTextKey);

        this.hasCachedOriginalSubMenuHelp = true;
    }

    private void CacheSingleSubMenuHelp(GameObject subMenuRoot, out String helpText, out String helpTextKey)
    {
        helpText = String.Empty;
        helpTextKey = String.Empty;

        if (subMenuRoot == null)
            return;

        ButtonGroupState buttonGroupState = subMenuRoot.GetComponent<ButtonGroupState>();
        if (buttonGroupState == null || buttonGroupState.Help == null)
            return;

        helpText = buttonGroupState.Help.Text;
        helpTextKey = buttonGroupState.Help.TextKey;
    }

    private void RestoreOriginalSubMenuHelp()
    {
        if (!this.hasCachedOriginalSubMenuHelp)
            return;

        this.RestoreSingleSubMenuHelp(this.BuySubMenu, this.originalBuySubMenuHelpText, this.originalBuySubMenuHelpTextKey);
        this.RestoreSingleSubMenuHelp(this.SellSubMenu, this.originalSellSubMenuHelpText, this.originalSellSubMenuHelpTextKey);
    }

    private void RestoreSingleSubMenuHelp(GameObject subMenuRoot, String helpText, String helpTextKey)
    {
        if (subMenuRoot == null)
            return;

        ButtonGroupState buttonGroupState = subMenuRoot.GetComponent<ButtonGroupState>();
        if (buttonGroupState == null || buttonGroupState.Help == null)
            return;

        buttonGroupState.Help.Text = helpText;
        buttonGroupState.Help.TextKey = helpTextKey;
    }
    public void RefreshDevItemInfoVisuals()
    {
        if (!this.isDevMode)
            return;

        this.CacheOriginalItemInfoCaptionsIfNeeded();
        this.GetItemFundCaptionLabel().rawText = String.Empty;
        this.itemFundLabel.rawText = String.Empty;
        this.GetItemCountCaptionLabel().rawText = "Possiedi:";
    }
    public void RefreshDevSubMenuLabels()
    {
        if (this.BuySubMenu != null)
        {
            UILabel buyLabel = this.FindSubMenuLabel(this.BuySubMenu);
            if (buyLabel != null)
                buyLabel.rawText = DevItemsTabLabel;
        }

        if (this.SellSubMenu != null)
        {
            UILabel sellLabel = this.FindSubMenuLabel(this.SellSubMenu);
            if (sellLabel != null)
                sellLabel.rawText = DevKeyItemsTabLabel;
        }

        if (this.devCardsSubMenu != null)
        {
            UILabel cardsLabel = this.FindSubMenuLabel(this.devCardsSubMenu);
            if (cardsLabel != null)
                cardsLabel.rawText = DevCardsTabLabel;
        }

        this.RefreshDevSubMenuHelp();
    }
    public void RefreshDevSubMenuHelp()
    {
        this.CacheOriginalSubMenuHelpIfNeeded();
        this.SetSubMenuHelp(this.BuySubMenu, DevItemsSubMenuHelp);
        this.SetSubMenuHelp(this.SellSubMenu, DevKeyItemsSubMenuHelp);
        this.SetSubMenuHelp(this.devCardsSubMenu, DevCardsSubMenuHelp);
    }

    private void SetSubMenuHelp(GameObject subMenuRoot, String helpText)
    {
        if (subMenuRoot == null)
            return;

        ButtonGroupState buttonGroupState = subMenuRoot.GetComponent<ButtonGroupState>();
        if (buttonGroupState == null || buttonGroupState.Help == null)
            return;

        buttonGroupState.Help.TextKey = String.Empty;
        buttonGroupState.Help.Text = helpText;
    }
    public class DevKeyItemListData : ListDataTypeBase
    {
        public Int32 Id;
        public Boolean Enable;
    }

    public class DevCardListData : ListDataTypeBase
    {
        public TetraMasterCardId Id;
        public Boolean Enable;
    }

    public void EnableDevMode()
    {
        this.isDevMode = true;
        this.type = ShopUI.ShopType.Item;
        this.currentMenu = ShopUI.SubMenu.Buy;
        this.currentDevCategory = DevMenuCategory.Items;
        this.devKeyItemIdList.Clear();
        this.devCardIdList.Clear();
        this.EnsureDevCardsSubMenuExists();
        this.ApplyDevThreeTabLayout();
        this.RefreshDevSubMenuLabels();
    }

    public void DisableDevMode()
    {
        this.isDevMode = false;
        this.RestoreOriginalSubMenuHelp();
        this.RestoreOriginalItemInfoCaptions();
        this.RestoreOriginalDevTabLayout();
        if (this.devCardsSubMenu != null)
            this.devCardsSubMenu.SetActive(false);
    }

    public void SetDevCategoryToKeyItems()
    {
        this.currentDevCategory = DevMenuCategory.KeyItems;
    }

    public void SetDevCategoryToItems()
    {
        this.currentDevCategory = DevMenuCategory.Items;
    }

    public void SetDevCategoryToCards()
    {
        this.currentDevCategory = DevMenuCategory.Cards;
    }

    private Boolean IsDevKeyItemCategory
    {
        get
        {
            return this.currentDevCategory == DevMenuCategory.KeyItems;
        }
    }

    private Boolean IsDevCardCategory
    {
        get
        {
            return this.currentDevCategory == DevMenuCategory.Cards;
        }
    }

    private void SyncDevCategoryFromCurrentMenu()
    {
        this.currentDevCategory =
            this.currentMenu == ShopUI.SubMenu.Sell
                ? DevMenuCategory.KeyItems
                : DevMenuCategory.Items;
    }

    private void EnsureDevCardsSubMenuExists()
    {
        if (this.devCardsSubMenu == null)
        {
            this.devCardsSubMenu = UnityEngine.Object.Instantiate(this.SellSubMenu) as GameObject;
            if (this.devCardsSubMenu != null)
                this.devCardsSubMenu.transform.SetParent(this.SellSubMenu.transform.parent, false);
            this.devCardsSubMenu.name = "Cards SubMenu";
            UIEventListener.Get(this.devCardsSubMenu).onClick += this.onClick;
        }

        this.devCardsSubMenu.SetActive(true);
    }

    private void ApplyDevThreeTabLayout()
    {
        this.EnsureDevCardsSubMenuExists();

        if (!this.hasCachedOriginalDevTabLayout)
        {
            this.originalBuySubMenuLocalPosition = this.BuySubMenu.transform.localPosition;
            this.originalSellSubMenuLocalPosition = this.SellSubMenu.transform.localPosition;
            this.originalBuySubMenuLocalScale = this.BuySubMenu.transform.localScale;
            this.originalSellSubMenuLocalScale = this.SellSubMenu.transform.localScale;
            this.hasCachedOriginalDevTabLayout = true;
        }

        Vector3 leftPosition = this.originalBuySubMenuLocalPosition;
        Vector3 rightPosition = this.originalSellSubMenuLocalPosition;
        Vector3 middlePosition = Vector3.Lerp(leftPosition, rightPosition, 0.5f);

        this.BuySubMenu.transform.localPosition = leftPosition;
        this.SellSubMenu.transform.localPosition = middlePosition;
        this.devCardsSubMenu.transform.localPosition = rightPosition;

        Vector3 buyScale = this.originalBuySubMenuLocalScale;
        Vector3 sellScale = this.originalSellSubMenuLocalScale;
        buyScale.x *= 0.66f;
        sellScale.x *= 0.66f;

        this.BuySubMenu.transform.localScale = buyScale;
        this.SellSubMenu.transform.localScale = sellScale;
        this.devCardsSubMenu.transform.localScale = sellScale;
    }

    private void RestoreOriginalDevTabLayout()
    {
        if (!this.hasCachedOriginalDevTabLayout)
            return;

        this.BuySubMenu.transform.localPosition = this.originalBuySubMenuLocalPosition;
        this.SellSubMenu.transform.localPosition = this.originalSellSubMenuLocalPosition;
        this.BuySubMenu.transform.localScale = this.originalBuySubMenuLocalScale;
        this.SellSubMenu.transform.localScale = this.originalSellSubMenuLocalScale;
    }

    private DevMenuCategory GetDevCategoryFromGameObject(GameObject go)
    {
        if (go == null)
            return this.currentDevCategory;

        Transform t = go.transform;

        while (t != null)
        {
            if (t.gameObject == this.BuySubMenu)
                return DevMenuCategory.Items;

            if (t.gameObject == this.SellSubMenu)
                return DevMenuCategory.KeyItems;

            if (t.gameObject == this.devCardsSubMenu)
                return DevMenuCategory.Cards;

            t = t.parent;
        }

        return this.currentDevCategory;
    }

    private Boolean TryInitializeDevData()
    {
        if (!this.isDevMode)
            return false;

        this.type = ShopUI.ShopType.Item;
        this.isGrocery = false;

        this.itemIdList.Clear();
        this.isItemEnableList.Clear();
        this.devKeyItemIdList.Clear();
        this.devCardIdList.Clear();

        if (this.IsDevKeyItemCategory)
        {
            for (Int32 id = 0; id < 256; id++)
            {
                String name = FF9TextTool.ImportantItemName(id);
                if (!String.IsNullOrEmpty(name))
                    this.devKeyItemIdList.Add(id);
            }
        }
        else if (this.IsDevCardCategory)
        {
            foreach (TetraMasterCardId cardId in Enum.GetValues(typeof(TetraMasterCardId)).Cast<TetraMasterCardId>().OrderBy(id => (Int32)id))
            {
                if (cardId == TetraMasterCardId.NONE)
                    continue;

                this.devCardIdList.Add(cardId);
            }
        }

        this.UpdateNavigateCharacterTooltip();
        return true;
    }

    private Boolean TryHandleDevSubMenuConfirm(GameObject go)
    {
        if (!this.isDevMode)
            return false;

        if (ButtonGroupState.ActiveGroup != ShopUI.SubMenuGroupButton)
            return false;

        FF9Sfx.FF9SFX_Play(103);

        this.currentDevCategory = this.GetDevCategoryFromGameObject(go);
        this.currentMenu = this.currentDevCategory == DevMenuCategory.KeyItems ? ShopUI.SubMenu.Sell : ShopUI.SubMenu.Buy;
        this.InitializeData();
        this.SetShopType(ShopUI.ShopType.Item);

        ButtonGroupState.RemoveCursorMemorize(ShopUI.ItemGroupButton);
        ButtonGroupState.ActiveGroup = ShopUI.ItemGroupButton;
        ButtonGroupState.SetSecondaryOnGroup(ShopUI.SubMenuGroupButton);
        ButtonGroupState.HoldActiveStateOnGroup(ShopUI.SubMenuGroupButton);

        this.currentItemIndex = 0;
        this.shopItemScrollList.JumpToIndex(0, false);
        this.DisplayInfo(ShopUI.ShopType.Item);

        this.RefreshDevSubMenuLabels();
        return true;
    }

    private Boolean TryHandleDevItemGroupConfirm(GameObject go)
    {
        if (!this.isDevMode)
            return false;

        if (ButtonGroupState.ActiveGroup != ShopUI.ItemGroupButton)
            return false;

        if (!ButtonGroupState.ContainButtonInGroup(go, ShopUI.ItemGroupButton))
            return false;

        this.currentItemIndex = go.GetComponent<RecycleListItem>().ItemDataIndex;

        Boolean canConfirm;

        if (this.IsDevKeyItemCategory)
        {
            canConfirm =
                this.currentItemIndex >= 0 &&
                this.currentItemIndex < this.devKeyItemIdList.Count &&
                !FF9StateSystem.Common.FF9.rare_item_obtained.Contains(this.devKeyItemIdList[this.currentItemIndex]);
        }
        else if (this.IsDevCardCategory)
        {
            canConfirm =
                this.currentItemIndex >= 0 &&
                this.currentItemIndex < this.devCardIdList.Count &&
                QuadMistDatabase.GetCardList().Count < Configuration.TetraMaster.MaxCardCount;
        }
        else
        {
            canConfirm =
                this.currentItemIndex >= 0 &&
                this.currentItemIndex < this.isItemEnableList.Count &&
                this.isItemEnableList[this.currentItemIndex];
        }

        if (!canConfirm)
        {
            FF9Sfx.FF9SFX_Play(102);
            return true;
        }

        FF9Sfx.FF9SFX_Play(103);
        if (this.IsDevKeyItemCategory)
        {
            this.count = 1;
            this.minCount = 1;
            this.maxCount = 1;
            this.isPlusQuantity = false;
            this.isMinusQuantity = false;
        }
        else if (this.IsDevCardCategory)
        {
            Int32 availableCardSlots = Math.Max(0, Configuration.TetraMaster.MaxCardCount - QuadMistDatabase.GetCardList().Count);
            this.count = availableCardSlots > 0 ? 1 : 0;
            this.minCount = availableCardSlots > 0 ? 1 : 0;
            this.maxCount = availableCardSlots;
            this.isPlusQuantity = false;
            this.isMinusQuantity = false;
        }
        else
        {
            this.StartCountItem();
        }
        this.DisplayConfirmDialog(this.Type);
        ButtonGroupState.ActiveGroup = ShopUI.QuantityGroupButton;
        ButtonGroupState.HoldActiveStateOnGroup(ShopUI.ItemGroupButton);
        return true;
    }

    private Boolean TryHandleDevQuantityConfirm()
    {
        if (!this.isDevMode)
            return false;

        if (ButtonGroupState.ActiveGroup != ShopUI.QuantityGroupButton)
            return false;

        this.ClearConfirmDialog();
        FF9Sfx.FF9SFX_Play(1045);

        if (this.IsDevKeyItemCategory)
        {
            Int32 keyItemId = this.devKeyItemIdList[this.currentItemIndex];
            ff9item.FF9Item_AddImportant(keyItemId);
        }
        else if (this.IsDevCardCategory)
        {
            TetraMasterCardId cardId = this.devCardIdList[this.currentItemIndex];
            for (Int32 i = 0; i < this.count; i++)
            {
                if (QuadMistDatabase.MiniGame_SetCard(cardId) == 0)
                    break;
            }
        }
        else
        {
            ff9item.FF9Item_Add(this.itemIdList[this.currentItemIndex], this.count);
        }

        this.DisplayItem();
        this.DisplayInfo(ShopUI.ShopType.Item);
        ButtonGroupState.ActiveGroup = ShopUI.ItemGroupButton;
        return true;
    }

    private Boolean TryHandleDevItemCancel()
    {
        if (!this.isDevMode)
            return false;

        if (ButtonGroupState.ActiveGroup != ShopUI.ItemGroupButton)
            return false;

        this.ClearInfo(ShopUI.ShopType.Item);
        ButtonGroupState.ActiveGroup = ShopUI.SubMenuGroupButton;
        return true;
    }

    private Boolean TryHandleDevQuantityCancel()
    {
        if (!this.isDevMode)
            return false;

        if (ButtonGroupState.ActiveGroup != ShopUI.QuantityGroupButton)
            return false;

        this.ClearConfirmDialog();
        ButtonGroupState.ActiveGroup = ShopUI.ItemGroupButton;
        return true;
    }

    private Boolean TryHandleDevSubMenuSelect(GameObject go)
    {
        if (!this.isDevMode)
            return false;

        if (ButtonGroupState.ActiveGroup != ShopUI.SubMenuGroupButton)
            return false;

        ButtonGroupState.HoldActiveStateOnGroup(go, ShopUI.SubMenuGroupButton);

        DevMenuCategory selectedCategory = this.GetDevCategoryFromGameObject(go);
        if (this.currentDevCategory == selectedCategory)
            return true;

        this.currentDevCategory = selectedCategory;
        this.currentMenu = this.currentDevCategory == DevMenuCategory.KeyItems ? ShopUI.SubMenu.Sell : ShopUI.SubMenu.Buy;
        this.InitializeData();
        this.currentItemIndex = -1;
        this.SetShopType(ShopUI.ShopType.Item);
        this.RefreshDevSubMenuLabels();
        return true;
    }

    private Boolean TryDisplayDevHelpPanel()
    {
        if (!this.isDevMode)
            return false;

        this.ShopTitleLabel.GetComponent<UILabel>().rawText = DevTitle;
        this.HelpLabel.GetComponent<UILabel>().rawText =
            this.IsDevKeyItemCategory
                ? DevKeyItemsHelp
                : this.IsDevCardCategory
                    ? DevCardsHelp
                    : DevItemsHelp;
        return true;
    }

    private Boolean TryDisplayDevInfo(ShopUI.ShopType shopType)
    {
        if (!this.isDevMode)
            return false;

        this.CacheOriginalItemInfoCaptionsIfNeeded();

        // Nasconde la voce "Guil"
        this.GetItemFundCaptionLabel().rawText = String.Empty;
        this.itemFundLabel.rawText = String.Empty;

        // Tiene visibile "Possiedi"
        this.GetItemCountCaptionLabel().rawText = "Possiedi:";

        if (this.IsDevKeyItemCategory)
        {
            if (this.currentItemIndex < 0 || this.currentItemIndex >= this.devKeyItemIdList.Count)
            {
                this.itemCountLabel.rawText = "-";
            }
            else
            {
                Boolean alreadyObtained =
                    FF9StateSystem.Common.FF9.rare_item_obtained.Contains(this.devKeyItemIdList[this.currentItemIndex]);

                this.itemCountLabel.rawText = alreadyObtained ? "Sì" : "No";
            }
        }
        else if (this.IsDevCardCategory)
        {
            this.itemCountLabel.rawText =
                this.currentItemIndex < 0 || this.currentItemIndex >= this.devCardIdList.Count
                    ? "-"
                    : QuadMistDatabase.MiniGame_GetCardCount(this.devCardIdList[this.currentItemIndex]).ToString();
        }
        else
        {
            this.itemCountLabel.rawText =
                this.currentItemIndex < 0 || this.currentItemIndex >= this.itemIdList.Count
                    ? "-"
                    : ff9item.FF9Item_GetCount(this.itemIdList[this.currentItemIndex]).ToString();
        }

        this.requiredItemsLabel.gameObject.SetActive(false);
        return true;
    }

    private Boolean TryDisplayDevItemList()
    {
        if (!this.isDevMode)
            return false;

        this.itemIdList.Clear();
        this.isItemEnableList.Clear();

        if (this.IsDevCardCategory)
        {
            List<ListDataTypeBase> cardList = new List<ListDataTypeBase>();
            Boolean hasFreeCardSlot = QuadMistDatabase.GetCardList().Count < Configuration.TetraMaster.MaxCardCount;

            foreach (TetraMasterCardId cardId in this.devCardIdList.OrderBy(id => (Int32)id))
            {
                cardList.Add(new ShopUI.DevCardListData
                {
                    Id = cardId,
                    Enable = hasFreeCardSlot
                });
            }

            if (this.shopItemScrollList.ItemsPool.Count == 0)
            {
                this.shopItemScrollList.PopulateListItemWithData = this.DisplayDevCardDetail;
                this.shopItemScrollList.OnRecycleListItemClick += this.OnListItemClick;
                this.shopItemScrollList.InitTableView(cardList, 0);
            }
            else
            {
                this.shopItemScrollList.PopulateListItemWithData = this.DisplayDevCardDetail;
                this.shopItemScrollList.SetOriginalData(cardList);
            }
        }
        else if (!this.IsDevKeyItemCategory)
        {
            List<ListDataTypeBase> list = new List<ListDataTypeBase>();

            foreach (RegularItem itemId in ff9item._FF9Item_Data.Keys.OrderBy(id => (Int32)id))
            {
                Int32 currentCount = ff9item.FF9Item_GetCount(itemId);
                Boolean isEnabled = currentCount < ff9item.FF9ITEM_COUNT_MAX;

                this.isItemEnableList.Add(isEnabled);
                this.itemIdList.Add(itemId);
                list.Add(new ShopUI.ShopItemListData
                {
                    Id = itemId,
                    Price = 0,
                    Enable = isEnabled
                });
            }

            if (this.shopItemScrollList.ItemsPool.Count == 0)
            {
                this.shopItemScrollList.PopulateListItemWithData = this.DisplayItemDetail;
                this.shopItemScrollList.OnRecycleListItemClick += this.OnListItemClick;
                this.shopItemScrollList.InitTableView(list, 0);
            }
            else
            {
                this.shopItemScrollList.PopulateListItemWithData = this.DisplayItemDetail;
                this.shopItemScrollList.SetOriginalData(list);
            }
        }
        else
        {
            List<ListDataTypeBase> keyItemList = new List<ListDataTypeBase>();

            foreach (Int32 keyItemId in this.devKeyItemIdList.OrderBy(id => id))
            {
                Boolean alreadyObtained = FF9StateSystem.Common.FF9.rare_item_obtained.Contains(keyItemId);

                keyItemList.Add(new ShopUI.DevKeyItemListData
                {
                    Id = keyItemId,
                    Enable = !alreadyObtained
                });
            }

            if (this.shopItemScrollList.ItemsPool.Count == 0)
            {
                this.shopItemScrollList.PopulateListItemWithData = this.DisplayDevKeyItemDetail;
                this.shopItemScrollList.OnRecycleListItemClick += this.OnListItemClick;
                this.shopItemScrollList.InitTableView(keyItemList, 0);
            }
            else
            {
                this.shopItemScrollList.PopulateListItemWithData = this.DisplayDevKeyItemDetail;
                this.shopItemScrollList.SetOriginalData(keyItemList);
            }
        }

        return true;
    }

    private void DisplayDevKeyItemDetail(Transform item, ListDataTypeBase data, Int32 index, Boolean isInit)
    {
        ShopUI.DevKeyItemListData devKeyItemListData = (ShopUI.DevKeyItemListData)data;
        ItemListDetailWithIconHUD itemListDetailWithIconHUD = new ItemListDetailWithIconHUD(item.gameObject, true);

        if (isInit)
            this.DisplayWindowBackground(item.gameObject, null);

        itemListDetailWithIconHUD.IconSprite.spriteName = String.Empty;
        itemListDetailWithIconHUD.IconSprite.gameObject.SetActive(false);
        itemListDetailWithIconHUD.NameLabel.rawText = FF9TextTool.ImportantItemName(devKeyItemListData.Id);
        itemListDetailWithIconHUD.NumberLabel.rawText = "-";

        if (devKeyItemListData.Enable)
        {
            itemListDetailWithIconHUD.NameLabel.color = FF9TextTool.White;
            itemListDetailWithIconHUD.NumberLabel.color = FF9TextTool.White;
            ButtonGroupState.SetButtonAnimation(itemListDetailWithIconHUD.Self, true);
        }
        else
        {
            itemListDetailWithIconHUD.NameLabel.color = FF9TextTool.Gray;
            itemListDetailWithIconHUD.NumberLabel.color = FF9TextTool.Gray;
            ButtonGroupState.SetButtonAnimation(itemListDetailWithIconHUD.Self, false);
        }

        itemListDetailWithIconHUD.Button.Help.TextKey = String.Empty;
        itemListDetailWithIconHUD.Button.Help.Text = FF9TextTool.ImportantItemHelpDescription(devKeyItemListData.Id);
    }


    private void DisplayDevCardDetail(Transform item, ListDataTypeBase data, Int32 index, Boolean isInit)
    {
        ShopUI.DevCardListData devCardListData = (ShopUI.DevCardListData)data;
        ItemListDetailWithIconHUD itemListDetailWithIconHUD = new ItemListDetailWithIconHUD(item.gameObject, true);

        if (isInit)
            this.DisplayWindowBackground(item.gameObject, null);

        itemListDetailWithIconHUD.IconSprite.spriteName = String.Empty;
        itemListDetailWithIconHUD.IconSprite.gameObject.SetActive(false);
        itemListDetailWithIconHUD.NameLabel.rawText = FF9TextTool.CardName(devCardListData.Id);
        itemListDetailWithIconHUD.NumberLabel.rawText = QuadMistDatabase.MiniGame_GetCardCount(devCardListData.Id).ToString();

        if (devCardListData.Enable)
        {
            itemListDetailWithIconHUD.NameLabel.color = FF9TextTool.White;
            itemListDetailWithIconHUD.NumberLabel.color = FF9TextTool.White;
            ButtonGroupState.SetButtonAnimation(itemListDetailWithIconHUD.Self, true);
        }
        else
        {
            itemListDetailWithIconHUD.NameLabel.color = FF9TextTool.Gray;
            itemListDetailWithIconHUD.NumberLabel.color = FF9TextTool.Gray;
            ButtonGroupState.SetButtonAnimation(itemListDetailWithIconHUD.Self, false);
        }

        itemListDetailWithIconHUD.Button.Help.TextKey = String.Empty;
        itemListDetailWithIconHUD.Button.Help.Text = FF9TextTool.CardName(devCardListData.Id);
    }

    private Boolean TryClearDevInfo(ShopUI.ShopType shopType)
    {
        if (!this.isDevMode)
            return false;

        if (shopType != ShopUI.ShopType.Item)
            return false;

        this.CacheOriginalItemInfoCaptionsIfNeeded();
        this.GetItemFundCaptionLabel().rawText = String.Empty;
        this.itemFundLabel.rawText = String.Empty;
        this.GetItemCountCaptionLabel().rawText = "Possiedi:";
        this.itemCountLabel.rawText = "-";
        return true;
    }
}
