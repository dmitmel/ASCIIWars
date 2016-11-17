using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
#if !DEBUG
using System.IO;
#endif

namespace ASCIIWars.ConsoleGraphics {
    public class Task {
        public readonly Func<string> nameSupplier;
        public readonly Action action;

        public string name { get { return nameSupplier(); } }

        public Task(string name, Action action) {
            nameSupplier = () => name;
            this.action = action;
        }

        public Task(Func<string> nameSupplier, Action action) {
            this.nameSupplier = nameSupplier;
            this.action = action;
        }

        public static Task EmptyTask(string name, int delay) {
            return new Task(name, () => Thread.Sleep(delay));
        }
    }

#if !DEBUG
    /**
     * @short Загружает задачи из файла со смешными названиями для задач. 
     *        В файле содержатся только имена, а класс преобразует их
     *        задачи со случайным временем от #MIN_TASK_DELAY
     *        до #MAX_TASK_DELAY.
     * @see Task
     * @see LoadingBar.Load
     */
    public static class FunnyTasksLoader {
        /// Минимальное время, которое может потробоваться на выполнение загруженой задачи.
        const int MIN_TASK_DELAY = 100;
        /// Максимальное время, которое может потробоваться на выполнение загруженой задачи.
        const int MAX_TASK_DELAY = 500;

        public static Task[] Load() {
            return File.ReadAllLines("assets/funny-task-names.txt").Select(taskName => {
                int taskDelay = GlobalRandom.Next(MIN_TASK_DELAY, MAX_TASK_DELAY);
                return Task.EmptyTask(taskName, taskDelay);
            }).ToArray();
        }
    }
#endif

    /**
     * @short Рисует символьную полоску загрузки.
     * 
     * - Загрузка выглядит примерно так:
     *   ```nohighlight
     *   Тут столько же решёток,
     *   На сколько процентов                 Тут столько же пробелов, сколько осталось до
     *   выполнены задачи                     завершения выполнения всех задач
     *             |                                                |
     *    _________|_________  _____________________________________|_______________________________________
     *   /                   \/                                                                             \
     *   #####################                                                                                21%
     *   Hello, World!                                                                                        \_/
     *   \___________/                                                                                         |
     *         |                                                                                      Процент выполнения
     *   Имя текущей задачи                                                                           всех задач
     *   ```
     * 
     * - Задач не должно быть больше чем 100. Если их меньше чем 100,
     *   то полоска будет завершена до 100.
     * 
     * - Когда выполнение закончится, то имя последней задачи
     *   будет заменено на значание #FINISHED_TITLE
     * 
     * - Консоль очищается, после того, как пройдёт время из
     *   #DELAY_BEFORE_CLEARING
     * 
     * @see Task
     */
    public static class LoadingBar {
        /// Строка, котороя будет выведена на месте последней задачи,
        /// при окончании загрузки.
        const string FINISHED_TITLE = "Загрузка завершена!";
        const int BAR_LENGTH = 100;
#if !DEBUG
        /// Время в миллисекундах, которое надо ждать, чтоб полоска
        /// загрузки очислилась, после окончания загрузки.
        const int DELAY_BEFORE_CLEARING = 1500;
#endif

        public static void Load(params Task[] tasks) {
            Load(tasks.ToList());
        }

        public static void Load(List<Task> tasks) {
            float completePercent = 0;

            foreach (Task task in tasks) {
                PrintProgressOf(task.name, completePercent);
                task.action.Invoke();
                completePercent += ((float) BAR_LENGTH) / tasks.Count;
            }

            PrintProgressOf(FINISHED_TITLE, 100.0f);

#if !DEBUG
            Thread.Sleep(100);
#endif
            // Полоска загрузки как раз занимает 2 линии
            MyConsole.ClearLine();  // Очищаем линию с текстом о завершении
            MyConsole.MoveCursorDown(1);
            MyConsole.ClearLine();  // Очищаем линию с полоской
            MyConsole.MoveCursorUp(1);
        }

        static void PrintProgressOf(string taskName, float completePercent) {
            int completePercentI = (int) Math.Ceiling(completePercent);

            Console.Write(new string('#', completePercentI));
            Console.Write(new string(' ', BAR_LENGTH - completePercentI));
            Console.WriteLine(string.Format(" {0:0.0}%", completePercent));
            MyConsole.ClearLine();
            Console.Write(taskName);
            MyConsole.MoveCursorUp(1);
            MyConsole.MoveCursorToBeginingOfLine();
        }
    }
}

