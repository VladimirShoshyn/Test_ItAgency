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

        internal void Read()
        {
            Thread.Sleep(ReadTimeSec * 1000);
            var addingData = rnd.Next();
            System.Console.WriteLine($"Adding {addingData}; queue count is {queue.Count}");
            queue.Enqueue(addingData);
        }

        internal void Write(int count)
        {   
            while (count > 0)
            {                
                if (queue.TryDequeue(out int savingItem))
                {
                    //Long saving data process
                    Thread.Sleep(WriteTimeSec * 1000);
                    System.Console.WriteLine($"wtited {savingItem}; queue count is {queue.Count}");
                    count--;
                }
            }            
        }

        internal void Quest(int count)
        {
            Task task = new Task(() => Write(count-1));
            task.Start();

            for (int i = 1; i < count; i++)
            {
                Read();                
            }            

            task.Wait();

            // Чтение и запись должны происходить одновременно.
            // Суммарное время должно быть примерно равно (кол-ву
            // повторов + 1) * большее из времени записи и чтения
        }
    }
}