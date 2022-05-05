using System;
using UnityEngine;

public class WTFExeption : Exception
{
    public override string Message => "WTF Message Exception";
}

public enum GameState
{
    None,
    Play,
    PlayerOneWin,
    PlayerTwoWin,
    EndGame,
}

public class Game
{
    private int[,] m_Field = new int[3, 3];
    private int m_Turn = 0;
    private GameState m_State;

    public event Action<int[,]> PlayerFieldChangeEvent;
    public event Action<GameState> GameStateChangeEvent;

    /// <summary>
    /// Current state of the game. Does not raise events if state has not changed on setter.
    /// Does not have any logic, just Prop to subscribe
    /// </summary>
    public GameState CurrentState
    {
        get => m_State;
        set
        {
            if (m_State != value)
            {
                m_State = value;
                GameStateChangeEvent?.Invoke(m_State);
            }
        }
    }

    /// <summary>
    /// Try to make turn at given cell
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void TryMakeTurn(int x, int y)
    {
        if (CurrentState != GameState.Play)
            return;

        var player = m_Turn % 2 == 0 ? 1 : 2;
        if (m_Field[x, y] == 0)
        {
            m_Turn++;
            m_Field[x, y] = player;
            PlayerFieldChangeEvent?.Invoke(m_Field);
            var winner = GetWinner(out _); //Fuck it, no time for printing result
            if (winner != 0)
            {
                CurrentState = winner switch
                {
                    -1 => GameState.EndGame,
                    1 => GameState.PlayerOneWin,
                    2 => GameState.PlayerTwoWin,
                    _ => throw new WTFExeption(),
                };
            }
        }
    }

    /// <summary>
    /// Try to get winner from the field.
    /// Game rules can be found here: CPECIFICAAAAAAATION (SRS) ;)
    /// https://manualzz.com/doc/47080478/tic-tac-toe
    /// 1 is player 1
    /// 2 is player 2
    /// 0 if no winner yet
    /// -1 if no winner
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    private int GetWinner(out Vector2Int[] result)
    {
        result = new Vector2Int[3]; //FU GC
        for (int i = 0; i < 3; i++)
        {
            var rowWinner = m_Field[i, 0] != 0 && m_Field[i, 0] == m_Field[i, 1] && m_Field[i, 0] == m_Field[i, 2];
            if (rowWinner)
            {
                result[0] = new Vector2Int(i, 0);
                result[1] = new Vector2Int(i, 1);
                result[2] = new Vector2Int(i, 2);
                return m_Field[i, 0];
            }

            var colWinner = m_Field[0, i] != 0 && m_Field[0, i] == m_Field[1, i] && m_Field[0, i] == m_Field[2, i];
            if (colWinner)
            {
                result[0] = new Vector2Int(0, i);
                result[1] = new Vector2Int(1, i);
                result[2] = new Vector2Int(2, i);
                return m_Field[0, i];
            }
        }
        if (m_Field[0, 0] != 0 && m_Field[0, 0] == m_Field[1, 1] && m_Field[0, 0] == m_Field[2, 2])
        {
            result[0] = new Vector2Int(0, 0);
            result[1] = new Vector2Int(1, 1);
            result[2] = new Vector2Int(2, 2);
            return m_Field[0, 0];
        }
        if (m_Field[2, 0] != 0 && m_Field[2, 0] == m_Field[1, 1] && m_Field[2, 0] == m_Field[0, 2])
        {
            result[0] = new Vector2Int(2, 0);
            result[1] = new Vector2Int(1, 1);
            result[2] = new Vector2Int(0, 2);
            return m_Field[2, 0];
        }

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (m_Field[i, j] == 0)
                    return 0;
        return -1;
    }

    /// <summary>
    /// Guess what doIdo 
    /// </summary>
    public void ResetGame()
    {
        CurrentState = GameState.None;

        m_Turn = 0;
        m_Field = new int[3, 3];
        var copy = (int[,])m_Field.Clone(); //FU GC X2
        PlayerFieldChangeEvent?.Invoke(copy);

        CurrentState = GameState.Play;
    }
}