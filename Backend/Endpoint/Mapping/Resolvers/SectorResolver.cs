using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GaiaProject.ViewModels.Board;
using GaiaProject.Engine.Model.Board;

namespace GaiaProject.Endpoint.Mapping.Resolvers
{
	public class SectorsResolver : IValueResolver<Map, MapViewModel, List<SectorViewModel>>
	{
		public List<SectorViewModel> Resolve(Map map, MapViewModel dest, List<SectorViewModel> destMember, ResolutionContext context)
		{
			var sectors = map.Hexes
				.GroupBy(hex => hex.SectorId)
				.Select(g =>
				{
					var firstHex = g.First();
					return new SectorViewModel
					{
						Id = g.Key,
						Number = firstHex.SectorNumber,
						Rotation = firstHex.SectorRotation,
						Row = g.Select(hex => hex.Row).Min(),
						Column = g.Select(hex => hex.Column).Min(),
						Hexes = g.Select(context.Mapper.Map<HexViewModel>).ToList()
					};
				})
				.ToList();
			return sectors;
		}
	}
}
