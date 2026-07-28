using System.Collections;
using rvb.scripts;
using UnityEngine;
using UnityEngine.UI;

public class UICtl : MonoBehaviour
{
    // 在 Inspector 中填写要显示的兵种配置 ID
    [SerializeField]
    private int[] roleIds;

    [SerializeField]
    private Font uiFont;

    private const int SpawnCount = 10;

    private SheepMgr sheepMgr;

    private Text redBossHpText;
    private Text blueBossHpText;

    private IEnumerator Start()
    {
        // 等待 SheepMgr 初始化
        while (SheepMgr.inc == null)
        {
            yield return null;
        }

        sheepMgr = SheepMgr.inc;

        if (uiFont == null)
        {
            uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        // 没配置兵种时，先使用默认兵种
        if (roleIds == null || roleIds.Length == 0)
        {
            roleIds = new[]
            {
                sheepMgr.sheepConfig.WarmUpID
            };
        }

        InitRoot();

        redBossHpText = CreateBossHpText(
            "RedBossHp",
            true
        );

        blueBossHpText = CreateBossHpText(
            "BlueBossHp",
            false
        );

        RectTransform redPanel = CreateButtonPanel(
            "RedButtonPanel",
            true
        );

        RectTransform bluePanel = CreateButtonPanel(
            "BlueButtonPanel",
            false
        );

        foreach (var roleIdValue in SheepRoleTypeInfos.All)
        {
           
            int roleId = roleIdValue.id;

            if (roleId==0) {
                continue;
            }

            string roleName = GetRoleName(roleId);

            CreateSpawnButton(
                redPanel,
                $"{roleName} ×10",
                SheepCamp.Red,
                roleId
            );

            CreateSpawnButton(
                bluePanel,
                $"{roleName} ×10",
                SheepCamp.Blue,
                roleId
            );
        }
    }

    private void Update()
    {
        if (sheepMgr == null || sheepMgr.bosses == null)
        {
            return;
        }

        UpdateBossHp(
            SheepCamp.Red,
            redBossHpText,
            "红方 Boss"
        );

        UpdateBossHp(
            SheepCamp.Blue,
            blueBossHpText,
            "蓝方 Boss"
        );
    }

    private void InitRoot()
    {
        RectTransform rectTransform = transform as RectTransform;

        if (rectTransform == null)
        {
            Debug.LogError("UICtl 必须挂在 Canvas 下的 UI 对象上");
            return;
        }

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private RectTransform CreateButtonPanel(
        string objectName,
        bool isLeft)
    {
        GameObject panelObject = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(Image),
            typeof(VerticalLayoutGroup),
            typeof(ContentSizeFitter)
        );

        panelObject.transform.SetParent(transform, false);

        RectTransform rectTransform =
            panelObject.GetComponent<RectTransform>();

        Vector2 anchor = isLeft
            ? new Vector2(0f, 0.5f)
            : new Vector2(1f, 0.5f);

        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.pivot = anchor;

        rectTransform.anchoredPosition = isLeft
            ? new Vector2(20f, 0f)
            : new Vector2(-20f, 0f);

        rectTransform.sizeDelta = new Vector2(220f, 0f);

        Image background = panelObject.GetComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0.35f);

        VerticalLayoutGroup layout =
            panelObject.GetComponent<VerticalLayoutGroup>();

        layout.padding = new RectOffset(10, 10, 10, 10);
        layout.spacing = 8f;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        ContentSizeFitter fitter =
            panelObject.GetComponent<ContentSizeFitter>();

        fitter.horizontalFit =
            ContentSizeFitter.FitMode.Unconstrained;

        fitter.verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;

        return rectTransform;
    }

    private void CreateSpawnButton(
        Transform parent,
        string buttonText,
        SheepCamp camp,
        int roleId)
    {
        GameObject buttonObject = new GameObject(
            buttonText,
            typeof(RectTransform),
            typeof(Image),
            typeof(Button),
            typeof(LayoutElement)
        );

        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.GetComponent<Image>();

        image.color = camp == SheepCamp.Red
            ? new Color(0.75f, 0.2f, 0.2f, 0.95f)
            : new Color(0.2f, 0.35f, 0.8f, 0.95f);

        LayoutElement layoutElement =
            buttonObject.GetComponent<LayoutElement>();

        layoutElement.preferredHeight = 30f;
        layoutElement.minHeight = 30f;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;

        button.onClick.AddListener(() =>
        {
            sheepMgr.produce_pets(
                roleId,
                SpawnCount,
                camp
            );
        });

        CreateButtonText(buttonObject.transform, buttonText);
    }

    private void CreateButtonText(
        Transform parent,
        string content)
    {
        GameObject textObject = new GameObject(
            "Text",
            typeof(RectTransform),
            typeof(Text)
        );

        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform =
            textObject.GetComponent<RectTransform>();

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Text text = textObject.GetComponent<Text>();
        text.font = uiFont;
        text.fontSize = 14;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.text = content;
        text.raycastTarget = false;
    }

    private Text CreateBossHpText(
        string objectName,
        bool isLeft)
    {
        GameObject textObject = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(Text)
        );

        textObject.transform.SetParent(transform, false);

        RectTransform rectTransform =
            textObject.GetComponent<RectTransform>();

        // 顶部中间
        rectTransform.anchorMin = new Vector2(0.5f, 1f);
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f);

        // 红方稍微靠左，蓝方稍微靠右
        rectTransform.anchoredPosition = isLeft
            ? new Vector2(-100f, -20f)
            : new Vector2(100f, -20f);

        rectTransform.sizeDelta = new Vector2(350f, 50f);

        Text text = textObject.GetComponent<Text>();
        text.font = uiFont;
        text.fontSize = 14;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;

        text.alignment = isLeft
            ? TextAnchor.UpperLeft
            : TextAnchor.UpperRight;

        return text;
    }

    private void UpdateBossHp(
        SheepCamp camp,
        Text hpText,
        string bossName)
    {
        if (hpText == null)
        {
            return;
        }

        int index = (int)camp;

        if (index < 0 ||
            index >= sheepMgr.bosses.Length ||
            sheepMgr.bosses[index] == null)
        {
            hpText.text = $"{bossName} HP：--";
            return;
        }

        float hp = Mathf.Max(
            0f,
            sheepMgr.bosses[index].curHp
        );

        hpText.text =
            $"{bossName} HP：{Mathf.CeilToInt(hp)}";
    }

    private string GetRoleName(int roleId)
    {
        SheepRoleTypeInfo roleInfo =
            SheepRoleTypeInfo.getById(roleId);

        if (roleInfo == null)
        {
            return roleId.ToString();
        }

        return roleInfo.name[0].ToString();
    }
}