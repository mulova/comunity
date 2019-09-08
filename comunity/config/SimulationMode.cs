
namespace mulova.comunity
{
	public enum SimulationMode {
		ReleaseBuild, DebugBuild, Editor
	}
	
	public static class SimulationModeEx {
		public static bool IsBuild(this SimulationMode mode) {
			return mode == SimulationMode.ReleaseBuild || mode == SimulationMode.DebugBuild;
		}
	}
}