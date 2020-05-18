namespace VirusTracker.ViewModels
{
    public class TrackerCookiesVM
    {
        public string Name { get; set; }
        public ContrivedValues Contrived { get; set; }


        public class NameRequest
        {
            public string Name { get; set; }
        }

        public class ContrivedValues
        {

            public string Name { get; set; }
            public string Phone { get; set; }

            public string Place { get; set; }
        }

    }
}
