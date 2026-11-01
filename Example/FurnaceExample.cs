using Core.SlotsSystem;

namespace Example
{
    public abstract class FurnaceItem
    {
        protected FurnaceItem(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public sealed class Fuel : FurnaceItem
    {
        public Fuel(string name) : base(name)
        {
        }
    }

    public sealed class Ash : FurnaceItem
    {
        public Ash(string name) : base(name)
        {
        }
    }

    public sealed class Furnace
    {
        private readonly Slot<FurnaceItem> _fuelSlot;
        private readonly SlotContainerAccess<FurnaceItem> _access = new();

        private bool _isWorking;

        public Furnace()
        {
            _fuelSlot = new Slot<FurnaceItem>(new SlotDefinition<FurnaceItem>());
            _access.Add(new SlotAccess<FurnaceItem>(_fuelSlot, CanInsert, CanExtract));
        }

        public ISlotContainerAccess<FurnaceItem> Access => _access;

        public void Start()
        {
            _isWorking = true;
        }

        public void Stop()
        {
            _isWorking = false;
        }

        public void BurnToAsh()
        {
            _fuelSlot.TrySet(new Ash("Ash"));
        }

        private bool CanInsert(FurnaceItem item)
        {
            return _isWorking == false && item is Fuel;
        }

        private bool CanExtract(FurnaceItem item)
        {
            return _isWorking == false;
        }
    }

    public static class FurnaceExample
    {
        public static void Run()
        {
            var furnace = new Furnace();
            var coal = new Fuel("Coal");

            furnace.Access.TryInsert(coal);

            furnace.Start();
            furnace.Access.TryExtract(item => item is Ash, out _);

            furnace.BurnToAsh();
            furnace.Stop();

            furnace.Access.TryExtract(item => item is Ash, out FurnaceItem ash);
        }
    }
}
