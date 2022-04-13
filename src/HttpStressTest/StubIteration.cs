using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace HttpStressTest
{
    public class StubIteration : IIteration
    {
        public TimeSpan IterationElapsedTime => throw new NotImplementedException();

        public Viki.LoadRunner.Engine.Core.Timer.Interfaces.ITimer Timer => throw new NotImplementedException();

        public object UserData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int GlobalIterationId => throw new NotImplementedException();

        public int ThreadIterationId => throw new NotImplementedException();

        public int ThreadId => -1;

        public void Checkpoint(string checkpointName = null)
        {
            throw new NotImplementedException();
        }

        public void SetError(object error)
        {
            throw new NotImplementedException();
        }
    }
}