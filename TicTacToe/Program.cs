enum Piece
{
	X, O
}

class Player (string name, Piece playerPiece)
{
	public string Name { get; set; } = name;
	public Piece piece { get; init; } = playerPiece;
}

class Board(int boardSize)
{
	private readonly string[,] _board = new string[boardSize, boardSize];
	private readonly int[] _rowValues = new int[boardSize];
	private readonly int[] _columnValues = new int[boardSize];
	private int _leftDiagonal;
	private int _rightDiagonal; 
	private int _totalMoves;

	public bool MakeMove(int rowIndex, int columnIndex, Player player)
	{
		if(IsValidIndex(rowIndex, columnIndex))
		{
			_board[rowIndex, columnIndex] = player.piece.ToString();
			_totalMoves += 1;
			if (player.piece == Piece.X)
			{
				_rowValues[rowIndex] += 1;
				_columnValues[columnIndex] += 1;
				if (rowIndex == columnIndex)
				{
					_leftDiagonal += 1;
				}

				if (rowIndex + columnIndex == boardSize-1)
				{
					_rightDiagonal += 1;
				}
			}
			else
			{
				_rowValues[rowIndex] -= 1;
				_columnValues[columnIndex] -= 1;
				if (rowIndex == columnIndex)
				{
					_leftDiagonal -= 1;
				}
				
				if (rowIndex + columnIndex == boardSize-1)
				{
					_rightDiagonal -= 1;
				}
			}

			return true;
		}

		Console.WriteLine("Wrong move!");
		return false;
	}

	private bool IsValidIndex(int rowIndex, int columnIndex)
	{
		return (rowIndex >= 0 && rowIndex < boardSize && columnIndex >= 0 && columnIndex < boardSize && _board[rowIndex, columnIndex] is null);
	}
	
	public bool IfFull()
	{
		return _totalMoves == (boardSize*boardSize);
	}
	
	public bool HasWinner()
	{
		foreach (int row in _rowValues)
		{
			if(row == boardSize || row == -boardSize)
			{
				return true;
			}
		}
		
		foreach (int column in _columnValues)
		{
			if(column == boardSize || column == -boardSize)
			{
				return true;
			}
		}

		if (_leftDiagonal == boardSize || _leftDiagonal == -boardSize)
		{
			return true;
		}
		
		if (_rightDiagonal == boardSize || _rightDiagonal == -boardSize)
		{
			return true;
		}

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

}

class Game(Player player_1, Player player_2, int boardSize)
{
	public Player player1 { get; init; } = player_1;
	public Player player2 { get; init; } = player_2;
	private Player current_player;
	private Board _board = new(boardSize);

	public void Play()
	{
		current_player = player1;
		while (!_board.IfFull() && !_board.HasWinner())
		{
			_board.PrintBoard();
			if (MakeMove())
			{
				if (_board.HasWinner())
				{
					Console.WriteLine($"Winner is {current_player.Name}");
					return;
				}

				SwitchPlayers();
			}
		}

		if (_board.IfFull())
		{
			Console.WriteLine("Its a draw!");
		}
	}

	private void SwitchPlayers()
	{
		current_player = (current_player == player1) ? player2 : player1;
	}

	private bool MakeMove()
	{
		Console.WriteLine($"{current_player.Name}'s Turn:");
		Console.WriteLine("Enter Row Index:");
		if (int.TryParse(Console.ReadLine(), out var rowIndex))
		{
			Console.WriteLine("Enter Column Index:");
			if (int.TryParse(Console.ReadLine(), out var columnIndex))
			{
				return _board.MakeMove(rowIndex, columnIndex, current_player);
			}
		}

		return false;
	}
}

class TicTacToe
{
	public static void Main(string[] args)
	{
		Player player1 = new Player("Player 1", Piece.X);
		Player player2 = new Player("Player 2", Piece.O);
		Game game = new(player1, player2, 4);
		game.Play();
	}
}