using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

//LINK234
//https://blog.unity.com/technology/whats-new-in-ui-toolkit
//https://forum.unity.com/threads/ui-toolkit-migration-guide.1138621/
//https://css-tricks.com/snippets/css/a-guide-to-flexbox/
//https://yogalayout.com/
//https://docs.unity3d.com/Manual/UIE-LayoutEngine.html
//https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-USS-Selectors-Pseudo-Classes.html

public class TicTacToeWindow : EditorWindow
{
    [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;
    [SerializeField] private VisualTreeAsset m_RowAsset = default;
    [SerializeField] private VisualTreeAsset m_CellAsset = default;

    private VisualElement[,] m_VisualCells;
    private Game m_Game;
    private GameState m_GameState;

    [MenuItem("Games/TicTacToe")]
    public static void ShowExample()
    {
        var wnd = GetWindow<TicTacToeWindow>();
        wnd.titleContent = new GUIContent("TicTacToe");
    }

    public void CreateGUI()
    {
        //I've got no roots but my home was never on the ground
        var gameRootContainer = m_VisualTreeAsset.Instantiate();
        gameRootContainer.style.flexGrow = 1;
        var gridRoot = gameRootContainer.Q("GridRoot");
        rootVisualElement.Add(gameRootContainer);

        //Create cells and connect input to them
        m_VisualCells = new VisualElement[3, 3];
        for (int i = 0; i < 3; i++)
        {
            var rowContainer = m_RowAsset.Instantiate();
            rowContainer.style.flexGrow = 1;
            gridRoot.Add(rowContainer);
            var row = rowContainer.Q("RowRoot");
            for (int j = 0; j < 3; j++)
            {
                var cellContainer = m_CellAsset.Instantiate();
                cellContainer.style.flexGrow = 1;
                row.Add(cellContainer);

                var cell = cellContainer.Q("Cell");
                m_VisualCells[i, j] = cell;
                var rowIndex = i;
                var colIndex = j;
                cell.RegisterCallback<ClickEvent>((_) => OnClick(rowIndex, colIndex));
            }
        }

        //Start game
        m_Game = new Game();
        m_Game.PlayerFieldChangeEvent += OnFieldDataChange;
        m_GameState = m_Game.CurrentState;
        m_Game.GameStateChangeEvent += OnGameStateChanged;
        m_Game.ResetGame();
    }

    private void OnGameStateChanged(GameState state)
    {
        var label = rootVisualElement.Q<TextElement>();
        label.text = state.ToString();
        if (state == GameState.None || state == GameState.Play)
            label.RemoveFromClassList("EndGameLabel");
        else
            label.AddToClassList("EndGameLabel");

        m_GameState = state;
    }

    private void OnClick(int x, int y)
    {
        if (m_GameState == GameState.Play)
            m_Game.TryMakeTurn(x, y);
        else
            m_Game.ResetGame();
    }

    private void OnFieldDataChange(int[,] cellData)
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                SetDataToCell(cellData[i, j], m_VisualCells[i, j]);
    }

    private void SetDataToCell(int data, VisualElement visualCell)
    {
        switch (data)
        {
            case 1:
                {
                    if (visualCell.Q<TickElement>() == null)
                        visualCell.Add(new TickElement());
                    visualCell.Q("Tac").AddToClassList("Hidden");
                }
                break;
            case 2:
                {
                    visualCell.Q<TickElement>()?.RemoveFromHierarchy();
                    visualCell.Q("Tac").RemoveFromClassList("Hidden");
                }
                break;
            case 0:
                {
                    visualCell.Q<TickElement>()?.RemoveFromHierarchy();
                    visualCell.Q("Tac").AddToClassList("Hidden");
                }
                break;
            default:
                throw new WTFExeption();
        }
    }
}
