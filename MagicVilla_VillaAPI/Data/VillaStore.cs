using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Data
{
    public static class VillaStore
    {
        public static List<VillaDTO> villaList = new List<VillaDTO> {
            new VillaDTO{ Id = 1,Name="Pool View", Occupancy = 8, Sqft = 2400},
            new VillaDTO{ Id = 2,Name="Beach View", Occupancy = 7, Sqft = 1600}
            };
    }
}
