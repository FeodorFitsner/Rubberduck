﻿using Rubberduck.Interaction.Navigation;
using Rubberduck.Parsing.Symbols;
using Rubberduck.VBEditor;

namespace Rubberduck.UI.Controls
{
    public class SearchResultItem : ViewModelBase, INavigateSource
    {
        private readonly NavigateCodeEventArgs _navigateArgs;
        private string _resultText;

        public SearchResultItem(Declaration parentScopeDeclaration, NavigateCodeEventArgs navigationInfo, string resultText, Selection? highlight = null)
        {
            _navigateArgs = navigationInfo;
            ParentScope = parentScopeDeclaration;
            _resultText = resultText;
            HighlightIndex = highlight;
        }

        public Declaration ParentScope { get; }

        public Selection Selection => _navigateArgs.Selection;

        public string ResultText
        {
            get => _resultText;
            set
            {
                if (_resultText == value)
                {
                    return;
                }

                _resultText = value;
                OnPropertyChanged();
            }
        }
        
        public Selection? HighlightIndex { get; }

        public NavigateCodeEventArgs GetNavigationArgs()
        {
            return _navigateArgs;
        }
    }
}
