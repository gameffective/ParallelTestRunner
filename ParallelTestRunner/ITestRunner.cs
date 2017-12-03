namespace ParallelTestRunner
{
    public interface ITestRunner
    {
        int ResultCode { get; }

        void Parse(bool isFilterMode);
        
        void Execute();
        
        void WriteTrx();
        
        void Clean();
    }
}