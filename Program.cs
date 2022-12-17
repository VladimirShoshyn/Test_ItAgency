using System;
using System.Collections.Concurrent;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var t = new Test();
            t.Quest(10);
        }        
    }

    internal class Test
    {
        private const int ReadTimeSec = 1;
        private const int WriteTimeSec = 2;

        private Random rnd = new Random();
        ConcurrentQueue<int> queue = new ConcurrentQueue<int>();

        internal Task ReadAsync(int counter)
        {            
            queue.Enqueue(rnd.Next());
            Thread.Sleep(ReadTimeSec * 1000);
            System.Console.WriteLine($"Data readed and sending to save process. Itteration {counter}. Queue count: {queue.Count}. Data: [{string.Join("; ", queue.ToList<int>())}]");
            
            return Task.CompletedTask;
        }

        internal void WriteData(object writeData)
        {
            //Long saving data process            
            Thread.Sleep(WriteTimeSec * 1000);
            queue.TryDequeue(out int res1);
            queue.TryDequeue(out int res2);
            System.Console.WriteLine($"Data [{res1}; {res2}] has been saved.");
        }

        internal async void Quest(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                if(queue.Count >= 2)
                {
                    i--;
                    continue;
                } 

                while(queue.Count < 2)
                {
                    await ReadAsync(i);
                }

                Thread myThread = new Thread(WriteData);
                myThread.Start(queue.ToList<int>());
            }

            // Чтение и запись должны происходить одновременно.
            // Суммарное время должно быть примерно равно (кол-ву
            // повторов + 1) * большее из времени записи и чтения
        }        
    }
}