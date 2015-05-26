using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;


namespace Mu6Sample
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public static class PositionMapping
    {
        public static Position GetPosition(ConsoleKeyInfo directionTyep)
        {
            switch (directionTyep.Key)
            {
                case ConsoleKey.UpArrow:
                    return new Position { X = -1, Y = 0 };
                case ConsoleKey.DownArrow:
                    return new Position { X = 1, Y = 0 };
                case ConsoleKey.RightArrow:
                    return new Position { X = 0, Y = 1 };
                case ConsoleKey.LeftArrow:
                    return new Position { X = 0, Y = -1 };
                case ConsoleKey.X:
                    return null;
                default:
                    throw new NotSupportedException("Not supported direction of type " + directionTyep);
            }
        }
    }
    public static class ObjectDrawer
    {
        public static void DrawObject(Position position)
        {
            Console.Clear();
            Console.SetCursorPosition(position.Y, position.X);
            Console.Write("*");
        }
    }

    public class DrawServer
    {
        private readonly ConcurrentQueue<Position> _queue;
        private readonly Position _currentPosition;
        private bool _isRunning;
        public DrawServer(ConcurrentQueue<Position> queue)
        {
            _queue = queue;
            _currentPosition = new Position { X = 0, Y = 0 };
        }

        public void Run()
        {
            _isRunning = true;
            while (_isRunning == true)
            {
                Position newDirection;
                if (_queue.TryDequeue(out newDirection) == false) 
                {
                    continue;
                }
                if (_currentPosition.X + newDirection.X >= 0
                    && _currentPosition.Y + newDirection.Y >= 0)
                {
                    _currentPosition.X += newDirection.X;
                    _currentPosition.Y += newDirection.Y;
                }
                ObjectDrawer.DrawObject(_currentPosition);
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Position nextDirection = PositionMapping.GetPosition(Console.ReadKey(true));
            var queue = new ConcurrentQueue<Position>();
            var drawServer = new DrawServer(queue);
            var serverThread = new Thread(() => drawServer.Run());
            serverThread.Start();
            while (true)
            {
                var newPosition = PositionMapping.GetPosition(Console.ReadKey(true));
                if (newPosition == null)
                {
                    drawServer.Stop();
                    break;
                }
                queue.Enqueue(newPosition);
            }
        }
    }
}
