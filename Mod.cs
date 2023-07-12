using Reloaded.Mod.Interfaces;
using nights.test.enemyanditemlightingfix.Template;
using nights.test.enemyanditemlightingfix.Configuration;
using Reloaded.Hooks.Definitions;
using CallingConventions = Reloaded.Hooks.Definitions.X86.CallingConventions;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Hooks.Definitions.Enums;

namespace nights.test.enemyanditemlightingfix;

/// <summary>
/// Your mod logic goes here.
/// </summary>
public class Mod : ModBase // <= Do not Remove.
{
	/// <summary>
	/// Provides access to the mod loader API.
	/// </summary>
	private readonly IModLoader _modLoader;

	/// <summary>
	/// Provides access to the Reloaded.Hooks API.
	/// </summary>
	/// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
	private readonly IReloadedHooks _hooks;

	/// <summary>
	/// Provides access to the Reloaded logger.
	/// </summary>
	private readonly ILogger _logger;

	/// <summary>
	/// Entry point into the mod, instance that created this class.
	/// </summary>
	private readonly IMod _owner;

	/// <summary>
	/// Provides access to this mod's configuration.
	/// </summary>
	private Config _configuration;

	/// <summary>
	/// The configuration of the currently executing mod.
	/// </summary>
	private readonly IModConfig _modConfig;

	public Mod(ModContext context) {
		_modLoader = context.ModLoader;
		_hooks = context.Hooks;
		_logger = context.Logger;
		_owner = context.Owner;
		_configuration = context.Configuration;
		_modConfig = context.ModConfig;

		unsafe {
			string[] asmCode = {
				$"use32",
				$"mov  ecx, [esi+8]", // in original assembly
				$"{_hooks.Utilities.GetAbsoluteCallMnemonics(RenderRenderable3DImpl, out RenderRenderable3DImplReverseWrapper)}"
			};
			if (_configuration.FixEnemyLighting) {
				RenderEnemyHook = _hooks.CreateAsmHook(asmCode, 0x4AECC0, AsmHookBehaviour.DoNotExecuteOriginal).Activate();
			}
			if (_configuration.FixItemLighting) {
				RenderItemHook = _hooks.CreateAsmHook(asmCode, 0x4AEC20, AsmHookBehaviour.DoNotExecuteOriginal).Activate();
			}
			RenderRenderable3DWrapper = _hooks.CreateWrapper<RenderRenderable3D>(0x4AF610, out _);
		}
	}

	[Function(CallingConventions.MicrosoftThiscall)]
	public unsafe delegate int RenderRenderable3D(byte* This);
	public IReverseWrapper<RenderRenderable3D> RenderRenderable3DImplReverseWrapper;
	public IAsmHook RenderEnemyHook;
	public IAsmHook RenderItemHook;
	public bool LightEnemies = true;
	public bool LightItems = true;
	public RenderRenderable3D RenderRenderable3DWrapper;
	public unsafe int RenderRenderable3DImpl(byte* This) {
		var lightAndSpecularEnable = (byte*)0x2509965;
		*lightAndSpecularEnable = 1;
		int result = RenderRenderable3DWrapper(This);
		*lightAndSpecularEnable = 0;
		return result;
	}

	#region Standard Overrides
	public override void ConfigurationUpdated(Config configuration)
	{
		// Apply settings from configuration.
		// ... your code here.
		_configuration = configuration;
		_logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
	}
	#endregion

	#region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public Mod() { }
#pragma warning restore CS8618
	#endregion
}
