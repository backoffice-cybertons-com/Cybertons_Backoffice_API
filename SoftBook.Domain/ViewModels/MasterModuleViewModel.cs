namespace SoftBook.Domain.ViewModels
{
    public class MasterModuleViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Controller { get; set; }

        public MasterModuleViewModel(string name, string description, string controller)
        {
            Name = name;
            Description = description;
            Controller = controller;
        }
    }
}
