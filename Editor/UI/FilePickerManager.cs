using System;
using System.Collections.Generic;
using ImGuiNET;
using WolfSharp.Core;

namespace Editor.UI
{
	public class FilePickerManager: Component
	{
		private static FilePickerManager _instance;

		private static FilePickerManager Instance
		{
			get
			{
				if (_instance == null)
				{
					var go = new GameObject();
					_instance = go.AddComponent<FilePickerManager>();
					Scene.ActiveScene.AddGameObject(go);
				}

				return _instance;
			}
		}

		private Action<string> _onFileChosen;
		private bool _isFolderPicker;
		private bool _isFilePicker;
		private List<string> _allowedExtensions;

		public static void PickFile(Action<string> onFileChosen)
		{
			if (Instance._isFilePicker || Instance._isFolderPicker)
			{
				Console.WriteLine("Error: file picker already active");
				return;
			}
			
			Instance._onFileChosen = onFileChosen;
			Instance._isFilePicker = true;
		}

		public static void PickFolder(Action<string> onFolderChosen)
		{
			if (Instance._isFilePicker || Instance._isFolderPicker)
			{
				Console.WriteLine("Error: file picker already active");
				return;
			}
			Instance._onFileChosen = onFolderChosen;
			Instance._isFolderPicker = true;
		}

		public override void Update()
		{
			if (_isFolderPicker)
			{
				ImGui.OpenPopup("pick-folder");
			}
			if (_isFilePicker)
			{
				ImGui.OpenPopup("pick-file");
			}
			
			if (ImGui.BeginPopupModal("pick-folder", ref _isFolderPicker, ImGuiWindowFlags.NoTitleBar))
			{
				var picker = FilePicker.GetFolderPicker(this, "/");
				var result = picker.Draw();
				switch (result)
				{
					case FilePickerResult.Finished:
						_onFileChosen?.Invoke(picker.SelectedFile);
						FilePicker.RemoveFilePicker(this);
						_isFolderPicker = false;
						break;
					case FilePickerResult.Cancelled:
						FilePicker.RemoveFilePicker(this);
						_isFolderPicker = false;
						break;
				}
				ImGui.EndPopup();
			}
			
			if (ImGui.BeginPopupModal("pick-file", ref _isFilePicker, ImGuiWindowFlags.NoTitleBar))
			{
				var picker = FilePicker.GetFilePicker(this, "/");
				picker.AllowedExtensions = _allowedExtensions;
				var result = picker.Draw();
				switch (result)
				{
					case FilePickerResult.Finished:
						_onFileChosen?.Invoke(picker.SelectedFile);
						FilePicker.RemoveFilePicker(this);
						_isFilePicker = false;
						break;
					case FilePickerResult.Cancelled:
						FilePicker.RemoveFilePicker(this);
						_isFilePicker = false;
						break;
				}
				ImGui.EndPopup();
			}
			ImGui.End();
		}
	}
}