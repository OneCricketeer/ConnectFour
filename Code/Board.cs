using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApplications.Connect4
{
    class Board
    {
        #region Fields
        List<List<char>> board = new List<List<char>>();
        private int width { get { return board[0].Count; } }
        private int height { get { return board.Count; } }
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Default constructor. Creates a 4x4 board
        /// </summary>
        public Board()
        {
            List<char> row = new List<char>(new char[] { '.', '.', '.', '.' });
            for (int i = 0; i < 4; i++)
            {
                board.Add(row);
            }
            //            width = board[0].Count;
            //            height = board.Count;
        }

        /// <summary>
        /// Creates a (row x col) board
        /// </summary>
        /// <param name="row">Height</param>
        /// <param name="col">Width</param>
        public Board(int row, int col)
        {
            List<char> r = new List<char>();
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    r.Add('.');
                }
                board.Add(r);
                r = new List<char>();
            }
            //            width = board[0].Count;
            //            height = board.Count;
        }

        #endregion Constructors

        #region Add
        // "Drops" a piece into a column
        public void add(Piece p, int col)
        {
            col--;
            // If out of bounds on either side, add to the outermost side
            if (col <= 0) col = 0;
            else if (col >= width) col = width - 1;

            char letter = p.ToString()[0];
            int row = 0;
            try
            {
                // "Gravity"
                while (board[row][col] == '.')
                {
                    row++;
                }
                board[row - 1][col] = letter;
            }
            catch (Exception)
            {
                // "Overflow catch" pushes the "bottom piece" "out" of the board
                if (row == 0)
                {
                    row = height - 1;
                    while (row > 0)
                    {
                        board[row][col] = board[row - 1][col];
                        row--;
                    }
                    board[0][col] = letter;

                }
                // "Stack" the letters
                else board[row - 1][col] = letter;
            }
        }

        public void add(int p, int col)
        {
            char c = Convert.ToString(p)[0];
            add(new Piece(c), col);
        }

        public void add(char c, int col)
        {
            add(new Piece(c), col);
        }

        public void add(String s, int col)
        {
            add(new Piece(s[0]), col);
        }

        #endregion Add

        // "Tilt" the board to the left to cause the pieces to move all the way left 
        public void tilt()
        {
            int index;
            foreach (List<char> row in board)
            {
                index = 0;
                for (int c = 0; c < width; c++)
                {
                    if (row[c] != '.')
                    {
                        if (row[index] == row[c])
                            index++;
                        else
                        {
                            row[index] = row[c];
                            row[c] = '.';
                            index++;
                        }
                    }
                }
            }
        }

        // Transposes the matrix
        public void transpose()
        {
            if (height == width)
            {
                char temp;

                // Swaps values over left diagonal
                for (int i = 0; i < board.Count - 1; i++)
                {
                    for (int j = i + 1; j < board.Count; j++)
                    {
                        temp = board[i][j];
                        board[i][j] = board[j][i];
                        board[j][i] = temp;
                        //                    board[i][j] = board[j][board.Count - i - 1];
                        //                    board[j][board.Count - i - 1] = temp;
                    }
                }
            }
            else
            {
                Board newboard = new Board(this.width, this.height);

                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < width; c++)
                    {
                        newboard.board[c][r] = board[r][c];
                    }
                }
                board = newboard.board;
            }
        }

        #region Rotate

        // "Rotate" the board 90 deg clockwise
        // Assumes an nxn board
        public void rotateCW(bool rotate = true)
        {
            if (rotate)
                transpose();
            //            Console.WriteLine("Transpose\n" + this);
            char temp;
            int maxdim = width < height ? width : height;
            // Swap columms
            foreach (List<char> row in board)
            {
                for (int j = 0; j < Math.Floor(width / 2.0); j++)
                {
                    //                    var lhs = row[j];
                    //                    var rhs = row[Math.Abs(j - (width - 1))];
                    temp = row[j];
                    row[j] = row[Math.Abs(j - (width - 1))];
                    row[Math.Abs(j - (width - 1))] = temp;
                    //                    swap(ref lhs, ref rhs);
                }
            }
        }

        // "Rotate" the board 90 deg counter-clockwise
        // Assumes an nxn board
        public void rotateCCW(bool rotate = true)
        {
            if (rotate)
                transpose();

            // Swap rows
            for (int i = 0; i < Math.Floor(height / 2.0); i++)
            {
                var temp = board[i];
                //                var bottom = board[Math.Abs(i - (height - 1))];
                board[i] = board[Math.Abs(i - (height - 1))];
                board[Math.Abs(i - (height - 1))] = temp;
                //                swap(ref top, ref bottom);
            }

        }

        // Rotate the board 90*rot deg clockwise
        // Assumes an nxn board
        public void rotateCW(int rot)
        {
            rot %= 4;
            if (rot < 0)
                rotateCCW(-rot);
            else
                for (int i = 0; i < rot; i++)
                {
                    rotateCW();
                }
        }

        // Rotate the board 90*rot deg counter-clockwise
        // Assumes an nxn board
        public void rotateCCW(int rot)
        {
            rot %= 4;
            if (rot < 0)
                rotateCW(-rot);
            else
                for (int i = 0; i < rot; i++)
                {
                    rotateCCW();
                }
        }

        #endregion Rotate

        #region Flip
        public void flipVert()
        {
            rotateCCW(false); // Reduces duplicate code
        }

        public void flipHor()
        {
            rotateCW(false); // Reduces duplicate code
        }
        #endregion Flip

        // Make pieces fall to the bottom of the board after rotate
        public void gravity()
        {
            int start;

            // foreach column
            for (int col = 0; col < width; col++)
            {
                start = height - 1;
                try
                {
                    while (board[start][col] == '.') { start--; } // Starting point to move to bottom
                }
                catch (Exception) { continue; } // the whole column is '.'

                // Start back at the bottom; run until top '.' is seen; move back up the column
                for (int bot = height - 1; board[start][col] != '.'; bot--)
                {
                    board[bot][col] = board[start][col]; // Swap the starting point and the bottom
                    board[start][col] = '.';
                    if (start != 0) start--; // Move the starting point up
                }
            }
        }


        private class Vector
        {
            private double[] v_arr = new double[2];
            public double x { get { return v_arr[0]; } }
            public double y { get { return v_arr[1]; } }

            public Vector()
            {
                v_arr[0] = 0;
                v_arr[1] = 0;
            }
            public Vector(int x, int y)
            {
                v_arr[0] = x;
                v_arr[1] = y;
            }

            public static Vector operator +(Vector v1, Vector v2)
            {
                for (int i = 0; i < 2; i++)
                {
                    v1.v_arr[i] += v2.v_arr[i];
                }
                return v1;
            }

            public static Vector operator -(Vector v1, Vector v2)
            {
                for (int i = 0; i < 2; i++)
                {
                    v1.v_arr[i] -= v2.v_arr[i];
                }
                return v1;
            }

            public void toUnitVector()
            {
                double x = Math.Pow(this.x, 2);
                double y = Math.Pow(this.y, 2);
                double denom = Math.Sqrt(x + y);
                for (int i = 0; i < 2; i++)
                {
                    v_arr[i] /= denom;
                }
            }
            public override string ToString()
            {
                return String.Format("<{0,2}, {1,2}>", this.x, this.y);
            }
            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                try
                {
                    Vector v2 = (Vector)obj;
                    return this.x == v2.x && this.y == v2.y;
                }
                catch (InvalidCastException) { return false; }
            }
        }

        private char? charAt(int i, int j, Vector v)
        {
            try
            {
                return board[(int)((double)i - v.y)][(int)(j + v.x)];
            }
            catch (Exception) { return null; }
        }
        private Vector inARowDirection(int r, int c)
        {


            char curr = board[r][c];

            for (int i = -1; i < 2; i++)
            {
                for (int j = 1; j > -2; j--)
                {
                    if (i == 0 && j == 0) continue;
                    try
                    {
                        char? ch = board[r + i][c + j];
                        if (ch == null) continue;
                        else if (curr == ch)
                        {
                            return new Vector(j, -i);
                        }
                    }
                    catch (Exception) { continue; }
                }
            }

            return new Vector();
        }

        // TODO: Check for n pieces in a row (default 4)
        public bool nInARow(int n = 4)
        {
            //            Console.WriteLine(inARowDirection(1, 1));

            int inARow;
            int row_index;
            int col_index;
            char? next;
            Vector null_vector = new Vector(0, 0);
            //            Vector prev = inARowDirection(row_index, col_index); // Start at the top

            Vector curr;
            for (int row = board.Count - 1; row >= 0; row--)
            {

                for (int col = 0; col < board[0].Count; col++)
                {
                    char c = board[row][col]; // Get the current character
                    curr = inARowDirection(row, col); // Get the vector of a surrounding character

                    if (!curr.Equals(null_vector))
                        next = charAt(row, col, curr); // Gets the next character in the direction of the vector
                    else continue;

                    inARow = 1;
                    row_index = 0;
                    col_index = 0;
                    while (c == charAt(row, col, curr))
                    {
                        if (inARow == 1) // Initializes row_index & col_index
                        {
                            row_index = row - (int)curr.y;
                            col_index = col + (int)curr.x;
                        }
                        else
                        {
                            row_index -= (int)curr.y;
                            col_index += (int)curr.x;
                        }

                        Console.Write("{3, 2} [{0} {1}] {2,4}\t", row_index, col_index, curr, board[row][col]);
                        inARow++; // self explanatory...
                        Console.WriteLine(inARow);

                        if (inARow == n) // also self explanatory
                        {
                            Console.WriteLine("{4} in a row starting at '{2}' [{0}, {1}] going {3}", row, col, board[row][col], curr, n);
                            return true;
                        }

                        if (next != null) // next == null when out of bounds
                        {
                            next = charAt(row_index, col_index, curr);
                            if (c != next) break;
                        }

                        else break;

                    }
                }
            }
            return false;

        }
        private void swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (List<char> row in board)
            {
                foreach (char c in row)
                {
                    sb.Append(c);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}