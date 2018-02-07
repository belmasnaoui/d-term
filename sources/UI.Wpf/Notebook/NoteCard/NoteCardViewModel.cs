﻿using AutoMapper;
using Notebook.Core;
using ReactiveUI;
using System;

namespace UI.Wpf.Notebook
{
	/// <summary>
	/// Represents a card view model for adding, editing and visualizing notes.
	/// </summary>
	public class NoteCardViewModel : ReactiveObject
	{
		//
		private Guid _id;
		private int _intex;
		private string _title;
		private string _description;
		private NoteViewModel _formData;
		private bool _isFlipped;

		//
		private readonly IMapper _mapper = null;
		private readonly INotebookRepository _notebookRepository = null;

		/// <summary>
		/// Constructor method.
		/// </summary>
		public NoteCardViewModel(IMapper mapper, INotebookRepository notebookRepository)
		{
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper), nameof(NoteCardViewModel));
			_notebookRepository = notebookRepository ?? throw new ArgumentNullException(nameof(notebookRepository), nameof(NoteCardViewModel));

			SetupCommands();
		}

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		public Guid Id
		{
			get => _id;
			set => this.RaiseAndSetIfChanged(ref _id, value);
		}

		/// <summary>
		/// Gets or sets the order index.
		/// </summary>
		public int Index
		{
			get => _intex;
			set => this.RaiseAndSetIfChanged(ref _intex, value);
		}

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		public string Title
		{
			get => _title;
			set => this.RaiseAndSetIfChanged(ref _title, value);
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		public string Description
		{
			get => _description;
			set => this.RaiseAndSetIfChanged(ref _description, value);
		}

		/// <summary>
		/// Add a new note.
		/// </summary>
		public ReactiveCommand AddCommand { get; protected set; }

		/// <summary>
		/// Edit a note.
		/// </summary>
		public ReactiveCommand EditCommand { get; protected set; }

		/// <summary>
		/// Delete a note.
		/// </summary>
		public ReactiveCommand DeleteCommand { get; protected set; }

		/// <summary>
		/// Cancel note add/edit.
		/// </summary>
		public ReactiveCommand CancelCommand { get; protected set; }

		/// <summary>
		/// Save note when adding/editing.
		/// </summary>
		public ReactiveCommand SaveCommand { get; protected set; }

		/// <summary>
		/// Cloned note data to use in the form.
		/// </summary>
		public NoteViewModel FormData
		{
			get => _formData;
			set => this.RaiseAndSetIfChanged(ref _formData, value);
		}

		/// <summary>
		/// Flags whick side of the card is being shown. If flipped, the add/edit form is visible.
		/// </summary>
		public bool IsFlipped
		{
			get => _isFlipped;
			set => this.RaiseAndSetIfChanged(ref _isFlipped, value);
		}

		/// <summary>
		/// Wire up commands with their respective actions.
		/// </summary>
		private void SetupCommands()
		{
			AddCommand = ReactiveCommand.Create(() =>
			{
				FormData = new NoteViewModel()
				{
					Id = Guid.NewGuid()
				};

				IsFlipped = true;
			});

			EditCommand = ReactiveCommand.Create(() =>
			{
				FormData = _mapper.Map<NoteViewModel>(this);

				IsFlipped = true;
			});

			DeleteCommand = ReactiveCommand.Create(() =>
			{
				System.Windows.MessageBox.Show(Title ?? "(Delete)");
			});

			CancelCommand = ReactiveCommand.Create(() =>
			{
				IsFlipped = false;
			});

			SaveCommand = ReactiveCommand.Create(() =>
			{
				if (Guid.Empty == Id)
				{
					System.Windows.MessageBox.Show(Title ?? "(Add)");
				}
				else
				{
					System.Windows.MessageBox.Show(Title ?? "(Edit)");
				}
			});
		}
	}
}
