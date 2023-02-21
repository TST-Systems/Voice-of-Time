/**
 * @author      - Timeplex
 * 
 * @created     - 09.02.2023
 * 
 * @last_change - 09.02.2023
 */
namespace VoTCore.Algorithms
{
    public class LoadingBar
    {
        public readonly int Steps;

        private int currentStep;
        public int CurrentStep { get => currentStep; }

        private bool IsActive = false;

        private int CurrentLenght = 0;

        public LoadingBar(int steps) 
        {
            Steps = steps;
        }

        public void Start()
        {
            if (IsActive) return;
            CurrentLenght = Console.WindowWidth;
            IsActive = true;
            Draw();
        }

        public void Update(int currentStep)
        {
            if (!IsActive) return;
            this.currentStep = currentStep;
            CurrentLenght = Console.WindowWidth;
            Draw();
        }

        public void Pause()
        {
            if (!IsActive) return;
            Clear();
        }

        public void Stop()
        {
            if (!IsActive) return;
            CurrentLenght = 0;
            Clear();
        }

        private void Draw()
        {
            if (!IsActive) return;
            var currentPercentage = (double) CurrentStep / Steps ;
            var preString = $"\r{currentPercentage * 100:F2}% [";
            var afterString = "]";

            var remainingChars = CurrentLenght - (preString.Length + afterString.Length);

            if (remainingChars < 0) { Clear();  Console.Write("?"); return; }

            var amountToSet = (int)Math.Round(remainingChars * currentPercentage);

            var fullString = preString;

            for(int i = 0; i < remainingChars; i++) 
            { 
                if(i < amountToSet)
                {
                    fullString += "=";
                    continue;
                }
                fullString += " ";
            }

            fullString += afterString;
            Console.Write(fullString);
        }

        private void Clear()
        {
            if (!IsActive) return;
            Console.Write('\r');

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(' ');
            }

            Console.Write('\r');
            IsActive = false;
        }
    }
}
