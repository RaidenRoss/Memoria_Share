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
    private const String DevItemsTabLabel = "Oggetti";
    private const String DevKeyItemsTabLabel = "Rarità";
    private const String DevItemsSubMenuHelp = "Per scegliere gli oggetti\nda aggiungere nell'inventario.";
    private const String DevKeyItemsSubMenuHelp = "Per scegliere le rarità\nda aggiungere nell'inventario.";
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
        KeyItems
    }

    private DevMenuCategory currentDevCategory = DevMenuCategory.Items;
    private readonly List<Int32> devKeyItemIdList = new List<Int32>();
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
        this.RefreshDevSubMenuHelp();
    }
    public void RefreshDevSubMenuHelp()
    {
        this.CacheOriginalSubMenuHelpIfNeeded();
        this.SetSubMenuHelp(this.BuySubMenu, DevItemsSubMenuHelp);
        this.SetSubMenuHelp(this.SellSubMenu, DevKeyItemsSubMenuHelp);
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

    public void EnableDevMode()
    {
        this.isDevMode = true;
        this.type = ShopUI.ShopType.Item;
        this.currentMenu = ShopUI.SubMenu.Buy;
        this.currentDevCategory = DevMenuCategory.Items;
        this.devKeyItemIdList.Clear();
        this.RefreshDevSubMenuLabels();
    }

    public void DisableDevMode()
    {
        this.isDevMode = false;
        this.RestoreOriginalSubMenuHelp();
        this.RestoreOriginalItemInfoCaptions();
    }

    public void SetDevCategoryToKeyItems()
    {
        this.currentDevCategory = DevMenuCategory.KeyItems;
    }

    public void SetDevCategoryToItems()
    {
        this.currentDevCategory = DevMenuCategory.Items;
    }

    private Boolean IsDevKeyItemCategory
    {
        get
        {
            return this.currentDevCategory == DevMenuCategory.KeyItems;
        }
    }

    private void SyncDevCategoryFromCurrentMenu()
    {
        this.currentDevCategory =
            this.currentMenu == ShopUI.SubMenu.Sell
                ? DevMenuCategory.KeyItems
                : DevMenuCategory.Items;
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

        if (this.IsDevKeyItemCategory)
        {
            for (Int32 id = 0; id < 256; id++)
            {
                String name = FF9TextTool.ImportantItemName(id);
                if (!String.IsNullOrEmpty(name))
                    this.devKeyItemIdList.Add(id);
            }
        }

        this.UpdateNavigateCharacterTooltip();
        return true;
    }

    private Boolean TryHandleDevSubMenuConfirm()
    {
        if (!this.isDevMode)
            return false;

        if (ButtonGroupState.ActiveGroup != ShopUI.SubMenuGroupButton)
            return false;

        FF9Sfx.FF9SFX_Play(103);

        this.SyncDevCategoryFromCurrentMenu();
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

        ShopUI.SubMenu selectedMenu = this.GetSubMenuFromGameObject(go);
        if (this.currentMenu == selectedMenu)
            return true;

        this.currentMenu = selectedMenu;
        this.SyncDevCategoryFromCurrentMenu();
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

        if (!this.IsDevKeyItemCategory)
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
