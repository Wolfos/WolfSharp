using System;
using System.IO;
using System.Numerics;
using ImGuiNET;
using WolfSharp.Core;

namespace Editor.UI
{
	public class EditorGUI : Component
	{
		private Vector4 _colour;
		private bool openFolderPicker;
		public override void Update()
		{
			if (ImGui.BeginMainMenuBar())
			{
				if (ImGui.BeginMenu("File"))
				{
					if (ImGui.MenuItem("New Scene", "ctrl+n"))
					{
						
					}
					if (ImGui.MenuItem("Open Scene", "ctrl+o"))
					{
						FilePickerManager.PickFile(OnOpenScene);
					}
					if (ImGui.MenuItem("Save Scene", "ctrl+s"))
					{
						
					}
					ImGui.EndMenu();
				}
				ImGui.EndMainMenuBar();
			}

			ImGui.Begin("Scene", ImGuiWindowFlags.MenuBar);
			if (ImGui.BeginMenuBar())
			{
				if (ImGui.BeginMenu("Create GameObject"))
				{
					if (ImGui.MenuItem("Empty"))
					{
						Editor.Scene.AddGameObject(new GameObject());
					}
					ImGui.EndMenu();
				}
				ImGui.EndMenuBar();
			}

			foreach (var gameObject in Editor.Scene.GameObjects)
			{
				ImGui.Button(gameObject.Name);
			}

			ImGui.End();
		}

		private void OnOpenScene(string sceneFile)
		{
			Console.WriteLine($"Opened new scene: {sceneFile}");
		}
	}
}