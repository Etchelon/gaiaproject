using System.Linq;
using AutoMapper;
using GaiaProject.ViewModels.Board;
using GaiaProject.Endpoint.Mapping.Converters;
using GaiaProject.Endpoint.Mapping.Resolvers;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Board;
using GaiaProject.ViewModels;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model.Players;
using GaiaProject.ViewModels.Users;
using GaiaProject.ViewModels.Players;
using System.Collections.Generic;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.ViewModels.Game;
using System;
using GaiaProject.Core.Model;
using GaiaProject.Engine.Model.Setup;

namespace GaiaProject.Endpoint.Mapping
{
	public class Profiles : Profile
	{
		private const int LogsPageSize = 20;

		public Profiles()
		{
			#region Map

			CreateMap<Building, BuildingViewModel>()
				.ForMember(dst => dst.Username, opt => opt.MapFrom<UsernameResolver, string>(src => src.PlayerId))
				.ForMember(dst => dst.FederationId, opt => opt.MapFrom((src, _, __, ctx) => GetBuildingsFederation(src, ctx)))
				;
			CreateMap<Hex, HexViewModel>()
				.BeforeMap((h, vm, ctx) => ctx.Items["Hex"] = h)
				.ForMember(dst => dst.Building, opt => opt.MapFrom((hex, hexVm, prop, ctx) => GetMainBuilding(hex, hex.Buildings, ctx)))
				.ForMember(dst => dst.LantidsParasiteBuilding, opt => opt.MapFrom((hex, hexVm, prop, ctx) => GetLantidsParasiteBuilding(hex.Buildings, hexVm, ctx)))
				.ForMember(dst => dst.IvitsSpaceStation, opt => opt.MapFrom((hex, hexVm, prop, ctx) => GetIvitsSpaceStation(hex, hex.Buildings, ctx)))
				.ForMember(dst => dst.Satellites, opt => opt.MapFrom((hex, hexVm, prop, ctx) => GetSatellites(hex, hex.Buildings, ctx)))
				.AfterMap((h, vm, ctx) => ctx.Items.Remove("Hex"))
				;
			CreateMap<Map, MapViewModel>()
				.ForMember(dst => dst.PlayerCount, opt => opt.MapFrom(src => src.ActualPlayerCount))
				.ForMember(dst => dst.Rows, opt => opt.MapFrom(src => GetRows(src)))
				.ForMember(dst => dst.Columns, opt => opt.MapFrom(src => GetColumns(src)))
				.ForMember(dst => dst.Sectors, opt => opt.MapFrom<SectorsResolver>())
				;

			#endregion

			#region Scoring Board

			CreateMap<ScoringTile, ScoringTileViewModel>()
				.ForMember(dst => dst.TileId, opt => opt.MapFrom(src => src.Id))
				.ForMember(dst => dst.Inactive, opt => opt.MapFrom<ScoringTileStatusResolver>())
				;
			CreateMap<FinalScoringState, FinalScoringStateViewModel>()
				.ForMember(dst => dst.TileId, opt => opt.MapFrom(src => src.Id))
				;
			CreateMap<FinalScoringState.PlayerState, FinalScoringStateViewModel.PlayerFinalScoringStatusViewModel>()
				.ConvertUsing<FinalScoringStateConverter>()
				;
			CreateMap<ScoringBoard, ScoringBoardViewModel>();

			#endregion

			#region Players

			CreateMap<User, UserViewModel>();
			CreateMap<PlayerInGame, PlayerInfoViewModel>()
				.ForMember(dst => dst.AvatarUrl, opt => opt.MapFrom<UserAvatarResolver, string>(src => src.Id))
				.ForMember(dst => dst.RaceName, opt => opt.MapFrom(src => RaceUtils.GetName(src.RaceId ?? Race.None)))
				.ForMember(dst => dst.Color, opt => opt.MapFrom(src => RaceUtils.GetColor(src.RaceId ?? Race.None)))
				.ForMember(dst => dst.Points, opt => opt.MapFrom(src => src.State != null ? src.State.Points : 10))
				.ForMember(dst => dst.IsActive, opt => opt.MapFrom((src, _, __, ctx) =>
					((GaiaProjectGame) ctx.Items["Game"]).ActivePlayerId == src.Id))
				;
			CreateMap<PlayerInGame, PlayerInGameViewModel>()
				.BeforeMap((p, vm, ctx) => ctx.Items["Player"] = p)
				.ForMember(dst => dst.IsActive, opt => opt.MapFrom((src, _, __, ctx) => ((GaiaProjectGame) ctx.Items["Game"]).ActivePlayerId == src.Id))
				.AfterMap((p, vm, ctx) => ctx.Items.Remove("Player"));
			CreateMap<PlayerState, PlayerStateViewModel>()
				.ForMember(dst => dst.TempTerraformingSteps, opt => opt.MapFrom(src => src.TempTerraformationSteps))
				.ForMember(dst => dst.NavigationRange, opt => opt.MapFrom(src => src.Range))
				.ForMember(dst => dst.AvailableGaiaformers, opt => opt.MapFrom(src => src.Gaiaformers.Where(g => g.Unlocked)))
				.ForMember(dst => dst.TechnologyTiles, opt => opt.MapFrom(src => GetTechnologyTiles(src)))
				.ForMember(dst => dst.Income, opt => opt.MapFrom<IncomeResolver>())
				.ForMember(dst => dst.HasPassed, opt => opt.MapFrom((ps, vm, _, ctx) => ((PlayerInGame)ctx.Items["Player"])!.HasPassed))
				.ForMember(dst => dst.ResearchAdvancements, opt => opt.MapFrom<PlayerResearchAdvancementsResolver>())
				.ForMember(dst => dst.PlanetaryInstituteActionSpace, opt => opt.MapFrom((src, dst, _, ctx) =>
				{
					var player = (PlayerInGame)ctx.Items["Player"];
					if (!player.State.Buildings.PlanetaryInstitute)
					{
						return null;
					}
					return player.RaceId switch
					{
						var t when
						t == Race.Ambas
						|| t == Race.Firaks
						|| t == Race.Ivits
						=> new SpecialActionSpaceViewModel
						{
							Kind = "planetary-institute",
							Type = t.Value,
							IsAvailable = !player.Actions.HasUsedPlanetaryInstitute
						},
						_ => null
					};
				}))
				.ForMember(dst => dst.RightAcademyActionSpace, opt => opt.MapFrom((src, dst, _, ctx) =>
				{
					var player = (PlayerInGame)ctx.Items["Player"];
					if (!player.State.Buildings.AcademyRight)
					{
						return null;
					}
					return new SpecialActionSpaceViewModel
					{
						Kind = "right-academy",
						Type = player.RaceId.Value,
						IsAvailable = !player.Actions.HasUsedRightAcademy
					};
				}))
				.ForMember(dst => dst.RaceActionSpace, opt => opt.MapFrom((src, dst, _, ctx) =>
				{
					var player = (PlayerInGame)ctx.Items["Player"];
					if (player.RaceId != Race.Bescods)
					{
						return null;
					}
					return new SpecialActionSpaceViewModel
					{
						Kind = "race",
						Type = Race.Bescods,
						IsAvailable = !player.Actions.HasUsedRaceAction
					};
				}))
				.ForMember(dst => dst.KnownPlanetTypes, opt => opt.MapFrom(src => src.KnownPlanetTypes.Count))
				.ForMember(dst => dst.GaiaPlanets, opt => opt.MapFrom(src => src.GaiaPlanets))
				.ForMember(dst => dst.ColonizedSectors, opt => opt.MapFrom(src => src.ColonizedSectors.Count))
				.ForMember(dst => dst.AdditionalInfo, opt => opt.MapFrom(GetAdditionalInfo))
				;
			CreateMap<Gaiaformer, GaiaformerViewModel>();
			CreateMap<PlayerResources, ResourcesViewModel>();
			CreateMap<PowerPools, PowerPoolsViewModel>()
				.ForMember(dst => dst.BrainstoneSummary, opt => opt.Ignore())
				;
			CreateMap<DeployedBuildings, DeployedBuildingsViewModel>();
			CreateMap<RoundBooster, RoundBoosterViewModel>()
				.ForMember(dst => dst.Inactive, opt => opt.Ignore())
				;
			CreateMap<FederationToken, FederationTokenViewModel>()
				.ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Type))
				;

			#endregion

			#region Research Board

			CreateMap<PowerActionSpace, PowerActionSpaceViewModel>();
			CreateMap<QicActionSpace, QicActionSpaceViewModel>();
			CreateMap<TechnologyTilePile, StandardTechnologyTileStackViewModel>();
			CreateMap<ResearchTrack, ResearchTrackViewModel>()
				.ForMember(dst => dst.Players, opt => opt.MapFrom<ResearchAdvancementsResolver>())
				.ForMember(dst => dst.AdvancedTileType, opt => opt.MapFrom(src => src.IsAdvancedTileAvailable ? src.AdvancedTileType : (AdvancedTechnologyTileType?)null))
				.ForMember(dst => dst.Federation, opt => opt.MapFrom(src => src.IsFederationTokenAvailable ? src.Federation : (FederationTokenType?)null))
				.ForMember(dst => dst.LostPlanet, opt => opt.MapFrom(src => src.IsLostPlanetAvailable))
				;
			CreateMap<ResearchBoard, ResearchBoardViewModel>();

			#endregion

			CreateMap<RoundBoosters.RoundBoosterTile, RoundBoosterTileViewModel>()
				.ForMember(dst => dst.Player, opt => opt.MapFrom<PlayerInfoResolver, string>(src => src.PlayerId))
				.ForMember(dst => dst.Used, opt => opt.Ignore())
				;
			CreateMap<Federations.FederationTokenPile, FederationTokenStackViewModel>();

			CreateMap<BoardState, BoardStateViewModel>()
				.ForMember(dst => dst.AvailableRoundBoosters, opt => opt.MapFrom(src => src.RoundBoosters.AvailableRoundBooster.Where(b => !b.IsTaken)))
				.ForMember(dst => dst.AvailableFederations, opt => opt.MapFrom(src => src.Federations.Tokens))
				;

			CreateMap<AuctionState, AuctionStateViewModel>()
				.ForMember(dst => dst.AuctionedRaces, opt => opt.MapFrom<AuctionedRacesResolver>())
				;

			CreateMap<GaiaProjectGame, GameBaseViewModel>()
				.ForMember(dst => dst.CurrentPhase, opt => opt.MapFrom(src => src.CurrentPhaseId))
				.ForMember(dst => dst.CreatedBy, opt => opt.MapFrom<UserResolver, string>(r => r.CreatedBy))
				;
			CreateMap<GaiaProjectGame, GameInfoViewModel>()
				.IncludeBase<GaiaProjectGame, GameBaseViewModel>()
				;
			CreateMap<GaiaProjectGame, GameStateViewModel>()
				.IncludeBase<GaiaProjectGame, GameBaseViewModel>()
				.ForMember(dst => dst.Players, opt => opt.MapFrom(
					src => src.Players.OrderBy(p => p.State != null ? p.State.CurrentRoundTurnOrder : p.InitialOrder))
				)
				.ForMember(dst => dst.ActivePlayer, opt => opt.Ignore())
				.ForMember(dst => dst.GameLogs, opt => opt.MapFrom(src => MapLogs(src)))
				.ForMember(dst => dst.AuctionState, opt => opt.MapFrom(src => src.Setup != null ? src.Setup.AuctionState : null))
				;
		}

		private int GetRows(Map map)
		{
			return map.Hexes.Select(hex => hex.Row).Max();
		}

		private int GetColumns(Map map)
		{
			return map.Hexes.Select(hex => hex.Column).Max();
		}

		private List<TechnologyTileViewModel> GetTechnologyTiles(PlayerState playerState)
		{
			return playerState.StandardTechnologyTiles.Select(standardTile =>
			{
				var covered = standardTile.CoveredByAdvancedTile;
				var coveringAdvancedTile = covered ? playerState.AdvancedTechnologyTiles.Single(advancedTile => advancedTile.CoveredTile == standardTile.Id) : null;
				return new TechnologyTileViewModel
				{
					Id = standardTile.Id,
					CoveredByAdvancedTile = coveringAdvancedTile?.Id,
					Used = coveringAdvancedTile?.Used ?? standardTile.Used
				};
			})
			.ToList();
		}

		private BuildingViewModel GetMainBuilding(Hex hex, Building[] buildings, ResolutionContext ctx)
		{
			if (!buildings.Any())
			{
				return null;
			}

			Building InnerGet()
			{
				switch (hex.ActualPlanetType)
				{
					// Empty space: there could be an ivits Space Station (together with other satellites)
					// Lost Planet: get the satellite
					case null:
						return null;
					case PlanetType.LostPlanet:
						return buildings.Single(b => b.Type == BuildingType.LostPlanet);
					// Transdim planet: get the Gaiaformer, if any
					case PlanetType.Transdim:
						return buildings.SingleOrDefault(b => b.Type == BuildingType.Gaiaformer);
					// Colonizable planet:
					default:
						var constructions = buildings.Where(b => b.Type <= BuildingType.Gaiaformer).ToArray();
						if (!constructions.Any())
						{
							return null;
						}

						if (constructions.Length == 2)
						{
							return constructions.First(b => b.RaceId != Race.Lantids);
						}

						return constructions.First();
				}
			}

			return ctx.Mapper.Map<BuildingViewModel>(InnerGet());
		}

		private BuildingViewModel GetLantidsParasiteBuilding(Building[] buildings, HexViewModel hexViewModel, ResolutionContext ctx)
		{
			var constructions = buildings.Where(b => b.Type <= BuildingType.AcademyRight).ToArray();
			var ret = constructions.Length == 2 ? constructions.FirstOrDefault(b => b.RaceId == Race.Lantids) : null;
			return ctx.Mapper.Map<BuildingViewModel>(ret);
		}

		private BuildingViewModel GetIvitsSpaceStation(Hex hex, Building[] buildings, ResolutionContext ctx)
		{
			if (hex.PlanetType.HasValue || !buildings.Any())
			{
				return null;
			}
			var spaceStation = buildings.FirstOrDefault(b => b.Type == BuildingType.IvitsSpaceStation);
			return ctx.Mapper.Map<BuildingViewModel>(spaceStation);
		}

		private List<BuildingViewModel> GetSatellites(Hex hex, Building[] buildings, ResolutionContext ctx)
		{
			if (hex.PlanetType.HasValue || !buildings.Any())
			{
				return new List<BuildingViewModel>();
			}
			var satellites = buildings.Where(b => b.Type == BuildingType.Satellite);
			return ctx.Mapper.Map<List<BuildingViewModel>>(satellites);
		}

		private string GetBuildingsFederation(Building building, ResolutionContext ctx)
		{
			var game = (GaiaProjectGame)ctx.Items["Game"];
			var hex = (Hex)ctx.Items["Hex"];
			var playerId = building.PlayerId;
			var player = game.GetPlayer(playerId);
			var hexFederation = player.State.Federations.SingleOrDefault(fed => fed.HexIds.Contains(hex.Id));
			return hexFederation?.Id;
		}

		private string GetAdditionalInfo(PlayerState state, PlayerStateViewModel _, string additionalInfo, ResolutionContext ctx)
		{
			var player = (PlayerInGame)ctx.Items["Player"];
			var isIvits = player.RaceId == Race.Ivits;
			if (!isIvits)
			{
				return null;
			}

			var buildings = player.State.Buildings;
			var hasStandardTile = player.State.StandardTechnologyTiles.SingleOrDefault(t => t.Id == StandardTechnologyTileType.PassiveBigBuildingsWorth4Power && !t.CoveredByAdvancedTile) != null;
			var totalPowerValueOfDeployedBuildings = buildings.Mines
				+ (buildings.TradingStations + buildings.ResearchLabs) * 2
				+ (1 + (buildings.AcademyLeft ? 1 : 0) + (buildings.AcademyRight ? 1 : 0)) * (hasStandardTile ? 4 : 3)
				+ buildings.IvitsSpaceStations;
			var powerOfFederation = player.State.Federations.SingleOrDefault()?.TotalPowerValue ?? 0;
			var powerOfBuildingsOutsideFederation = totalPowerValueOfDeployedBuildings - powerOfFederation;
			return $"{powerOfFederation} - {powerOfBuildingsOutsideFederation}";
		}

		private List<GameLogViewModel> MapLogs(GaiaProjectGame game)
		{
			string FormatDate(DateTime date) => date.ToString("dd/MM/yyyy hh:mm");
			string GetPlayerUsername(string id) => game.GetPlayer(id)?.Username ?? "ERROR";

			var logs = game.GameLogs;
			var ordered = logs.OrderBy(l => l.Timestamp).ToList();
			var uniqueActionId = -1;
			var byTurn = MoreLinq.Extensions.GroupAdjacentExtension.GroupAdjacent(ordered, l => $"{l.Turn ?? -1}_{(l.ActionId.HasValue ? l.ActionId.Value : uniqueActionId--)}").ToList();
			var ret = byTurn.Select(g =>
				{
					var nLogs = g.Count();
					var first = g.FirstOrDefault(l => l.Important) ?? g.First();
					if (first.IsSystem)
					{
						return new GameLogViewModel
						{
							IsSystem = true,
							Important = first.Important,
							Message = first.Message,
							Timestamp = FormatDate(first.Timestamp)
						};
					}

					var subLogs = (first.SubLogs ?? new List<GameLog>())
						.Concat(g.Skip(1).SelectMany(sl =>
						{
							var ret = new List<GameLog> { sl };
							return sl.SubLogs != null
								? ret.Concat(sl.SubLogs)
								: ret;
						}));
					return new GameLogViewModel
					{
						IsSystem = false,
						Important = first.Important,
						Message = first.Message,
						Timestamp = FormatDate(first.Timestamp),
						Turn = first.Turn,
						ActionId = first.ActionId,
						Player = GetPlayerUsername(first.PlayerId),
						Race = first.Race,
						SubLogs = subLogs.Select(sl => new GameSubLogViewModel
						{
							Message = sl.Message,
							Timestamp = FormatDate(sl.Timestamp),
							Player = GetPlayerUsername(sl.PlayerId),
							Race = sl.Race,
						})
						.ToList()
					};
				})
				.TakeLast(LogsPageSize)
				.ToList();
			return ret;
		}
	}
}
