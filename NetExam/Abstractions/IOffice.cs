namespace NetExam.Abstractions
{
    using System.Collections.Generic; // added for linq 
    public interface IOffice
    {
        string LocationName { get; }
        string Name { get; }
        int MaxCapacity { get; } // added for linq 
        IEnumerable<string> AvailableResources { get; } // added for linq 
    }
}