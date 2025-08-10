enum Piece
{
	X = 1, O = -1
}

interface IPlayingStrategy
{
	public List<int> GetMove(Board board, int boardSize);
}

abstract class Player
{
	public abstract Piece piece { get; init; }
	public abstract IPlayingStrategy strategy { get; set; }
	public abstract string Name { get; set; }
}

class HumanPlayingStrategy : IPlayingStrategy
{
	public List<int> GetMove(Board board, int boardSize)
	{
		Console.WriteLine("Enter row index:");
		int rowIndex = int.TryParse(Console.ReadLine(), out var parsedRow) ? parsedRow : -1;
		Console.WriteLine("Enter column index:");
		int columnIndex = int.TryParse(Console.ReadLine(), out var parsedColumn) ? parsedColumn : -1;
		if(rowIndex>=0 && rowIndex<boardSize && columnIndex>=0 && columnIndex<boardSize && board.IsCellEmpty(rowIndex, columnIndex))
			return [rowIndex, columnIndex];
		return [];
	}
}

class EasyPlayingStrategy : IPlayingStrategy
{
	public List<int> GetMove(Board board, int boardSize)
	{
		for (int rowIndex = 0; rowIndex < boardSize; rowIndex++)
		{
			for (int column = 0; column < boardSize; column++)
			{
				if (board.IsCellEmpty(rowIndex, column))
				{
					return [rowIndex, column];
				}
			}
		}
		return [];
	}
}

class HumanPlayer(string name, Piece playerPiece, IPlayingStrategy playerStrategy) : Player
{
	public override Piece piece { get; init; } = playerPiece;
	public override IPlayingStrategy strategy { get; set; } = playerStrategy;
	public override string Name { get; set; } = name;
}

class AIPlayer(Piece playerPiece, IPlayingStrategy playerStrategy) : Player
{
	public override Piece piece { get; init; } = playerPiece;
	public override IPlayingStrategy strategy { get; set; } = playerStrategy;
	public override string Name { get; set; } = "AI";

	public void SetStrategy(IPlayingStrategy playingStrategy)
	{
		strategy = playingStrategy;
	}
}

class Board(int boardSize)
{
	private string[,] _board = new string[boardSize, boardSize];
	private int total_moves;
	private int[] rowValues = new int[boardSize];
	private int[] columnValues = new int[boardSize];
	private int leftDiagonal;
	private int rightDiagonal;

	private bool IsValidMove(int rowIndex, int columnIndex)
	{
		return (rowIndex >= 0 && rowIndex < boardSize && columnIndex >= 0 && columnIndex < boardSize);
	}

	public bool MakeMove(int rowIndex, int columnIndex, Piece piece)
	{
		if (IsValidMove(rowIndex, columnIndex) && IsCellEmpty(rowIndex, columnIndex))
		{
			_board[rowIndex, columnIndex] = piece.ToString();
			rowValues[rowIndex] += (int) piece;
			columnValues[columnIndex] += (int) piece;
			total_moves += 1;
			if (rowIndex == columnIndex)
			{
				leftDiagonal += (int)piece;
			}

			if (rowIndex + columnIndex == boardSize - 1)
			{
				rightDiagonal += (int)piece;
			}
			return true;
		}
		Console.WriteLine("Invalid move!");
		return false;
	}

	public void PrintBoard()
	{
		Console.WriteLine("------------");
		for (int rowIndex = 0; rowIndex < boardSize; rowIndex++)
		{
			for(int columnIndex = 0; columnIndex < boardSize; columnIndex++)
			{
				Console.Write($" {_board[rowIndex, columnIndex] ?? "N"} |");
			}
			Console.WriteLine();
		}
		Console.WriteLine("------------");
	}

	public bool IsCellEmpty(int rowIndex, int columnIndex)
	{
		return _board[rowIndex, columnIndex] is null;
	}

	public bool IsBoardFull()
	{
		return total_moves == (boardSize*boardSize);
	}

	public bool HasWinner()
	{
		if (leftDiagonal == boardSize || leftDiagonal == -boardSize || rightDiagonal == boardSize || rightDiagonal == -boardSize)
		{
			return true;
		}

		for (int rowIndex = 0; rowIndex < boardSize; rowIndex++)
		{
			int rowValue = rowValues[rowIndex];
			if (rowValue == boardSize || rowValue == -boardSize)
			{
				return true;
			}
		}
		
		for (int columnIndex = 0; columnIndex < boardSize; columnIndex++)
		{
			int columnValue = columnValues[columnIndex];
			if (columnValue == boardSize || columnValue == -boardSize)
			{
				return true;
			}
		}

		return false;
	}

}

class Game(int boardSize, Player player1, Player player2)
{
	private Board _board = new (boardSize);
	private Player Player1 { get; init; } = player1;
	private Player Player2 { get; init; } = player2;
	private Player _currentPlayer = player1;
	
	private void SwitchPlayer()
	{
		_currentPlayer = _currentPlayer == Player1 ? Player2 : Player1;
	}
	
	public void MakeMove()
	{
		while (!_board.IsBoardFull() && !_board.HasWinner())
		{
			// Console.WriteLine($"{_currentPlayer.Name}'s Turn:");
			var move = _currentPlayer.strategy.GetMove(_board, boardSize);
			if (move.Count > 0)
			{
				int rowIndex = move[0];
				int columnIndex = move[1];
				Console.WriteLine($"{_currentPlayer.Name} played: {rowIndex}, {columnIndex}");
				if (_board.MakeMove(rowIndex, columnIndex, _currentPlayer.piece))
				{
					if (_board.HasWinner())
					{
						Console.WriteLine($"Winner is {_currentPlayer.Name}");
						return;
					}
					if (_board.IsBoardFull())
					{
						Console.WriteLine("Draw!");
						return;
					}

					_board.PrintBoard();
					SwitchPlayer();
				}
			}
		}
	}
}

class TicTacToe
{
	public static void Main(string[] args)
	{
		Player humanPlayer = new HumanPlayer("Player 1", Piece.X, new HumanPlayingStrategy());
		Player aiPlayer1 = new AIPlayer(Piece.O, new EasyPlayingStrategy());
		Player aiPlayer2 = new AIPlayer(Piece.X, new EasyPlayingStrategy());
		// Game game = new Game(3, humanPlayer, aiPlayer1);
		Game game = new Game(3, aiPlayer2, aiPlayer1);
		game.MakeMove();
	}
}