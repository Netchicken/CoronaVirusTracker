using System;

namespace VirusTracker.ViewModels
{
    //inheriting class
    public class TrackerCookiesVM
    {
        public string Name { get; set; }
        public ContrivedValues Contrived { get; set; }
    }
    //simple class
    public class NameRequest
    {
        public string Name { get; set; }
    }

    //all data class
    public class ContrivedValues
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }

        public string Place { get; set; }
    }

}
