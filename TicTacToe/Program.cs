enum Piece
{
	X, O
}

class Player (string name, Piece playerPiece)
{
	public string Name { get; set; } = name;
	public Piece piece { get; init; } = playerPiece;
}

class Board
{
	private string[,] _board = new string[3, 3];
	private int[] _rowValues = new int[3];
	private int[] _columnValues = new int[3];
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
			}
			else
			{
				_rowValues[rowIndex] -= 1;
				_columnValues[columnIndex] -= 1;
			}

			return true;
		}

		Console.WriteLine("Wrong move!");
		return false;
	}

	private bool IsValidIndex(int rowIndex, int columnIndex)
	{
		return (rowIndex >= 0 && rowIndex < 3 && columnIndex >= 0 && columnIndex < 3 && _board[rowIndex, columnIndex] is null);
	}
	
	public bool IfFull()
	{
		return _totalMoves == 9;
	}
	
	public bool HasWinner()
	{
		foreach (int row in _rowValues)
		{
			if(row == 3 || row == -3)
			{
				return true;
			}
		}
		
		foreach (int column in _columnValues)
		{
			if(column == 3 || column == -3)
			{
				return true;
			}
		}

		if (!(_board[1, 1] is null))
		{
			string current_piece = _board[1, 1];
			
			//left diagonal
			if (!(_board[0, 0] is null) && !(_board[2, 2] is null) && _board[0, 0] == current_piece && _board[2, 2] == current_piece)
			{
				return true;
			}
			
			//right diagonal
			if (!(_board[0, 2] is null) && !(_board[2, 0] is null) && _board[0, 2] == current_piece && _board[2, 0] == current_piece)
			{
				return true;
			}
			
		}

		return false;
	}

	public void PrintBoard()
	{
		for (int rowIndex = 0; rowIndex < 3; rowIndex++)
		{
			Console.WriteLine($"{_board[rowIndex, 0] ?? "N"} | {_board[rowIndex, 1] ?? "N"} | {_board[rowIndex, 2] ?? "N"}");
		}
		Console.WriteLine("-------------------");
	}

}

class Game(Player player_1, Player player_2)
{
	public Player player1 { get; init; } = player_1;
	public Player player2 { get; init; } = player_2;
	private Player current_player;
	private Board _board = new();

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
		Game game = new(player1, player2);
		game.Play();
	}
}