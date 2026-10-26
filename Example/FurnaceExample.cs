using Core.SlotsSystem;

namespace Example
{
    public sealed class Fuel
    {
        public Fuel(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public sealed class Furnace
    {
        private readonly Slot<Fuel> _fuelSlot;
        private readonly SlotAccess<Fuel> _fuelAccess;

        private bool _isWorking;

        public Furnace()
        {
            _fuelSlot = new Slot<Fuel>(new SlotDefinition<Fuel>());
            _fuelAccess = new SlotAccess<Fuel>(_fuelSlot, _ => _isWorking == false, _ => _isWorking == false);
        }

        public ISlotAccess<Fuel> FuelAccess => _fuelAccess;

        public void Start()
        {
            _isWorking = true;
        }

        public void Stop()
        {
            _isWorking = false;
        }

        public void BurnToAsh(Fuel ash)
        {
            _fuelSlot.TrySet(ash);
        }
    }

    public static class FurnaceExample
    {
        public static void Run()
        {
            var furnace = new Furnace();
            var coal = new Fuel("Coal");
            var ash = new Fuel("Ash");

            furnace.FuelAccess.TryInsert(coal);
            furnace.Start();

            furnace.FuelAccess.TryExtract(out _);
            furnace.BurnToAsh(ash);

            furnace.Stop();
            furnace.FuelAccess.TryExtract(out Fuel extracted);
        }
    }
}
