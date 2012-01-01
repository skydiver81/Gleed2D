using System ;
using System.Collections.Generic ;
using System.Linq ;
using Gleed2D.Core ;
using Microsoft.Xna.Framework ;
using Microsoft.Xna.Framework.Input ;
using StructureMap ;

namespace GLEED2D
{
	internal class InputHandlerWhenScalingItems : IHandleEditorInput
	{
		readonly IEditor _editor ;
		readonly IModel _model ;

		public InputHandlerWhenScalingItems(IEditor editor )
		{
			_editor = editor ;
			_model = IoC.Model ;
		}

		public void Update( )
		{
			IEnumerable<ItemEditor> allSelectedEditors = selectedEditors( ).ToList(  ) ;
			
			Vector2 newDistance = MouseStatus.WorldPosition - allSelectedEditors.First().ItemProperties.Position ;
			
			float factor = newDistance.Length( ) / _editor.GrabPoint.Length( ) ;
			
			int i = 0 ;

			allSelectedEditors.Where( si => si.CanScale ).ForEach(
				thingToScale =>
					{

						//todo: reimplement texture
						//if (selitem is TextureItem)
						//{
						//    _mainForm.SetToolStripStatusLabel1("Hold down [X] or [Y] to limit scaling to the according dimension.");
						//}

						Vector2 newscale = _editor.ScalesBeforeUserInteraction[ i ] ;
						if( !KeyboardStatus.IsKeyDown( Keys.Y ) )
						{
							newscale.X = _editor.ScalesBeforeUserInteraction[ i ].X * ( ( ( factor - 1.0f ) * 0.5f ) + 1.0f ) ;
						}

						if( !KeyboardStatus.IsKeyDown( Keys.X ) )
						{
							newscale.Y = _editor.ScalesBeforeUserInteraction[ i ].Y * ( ( ( factor - 1.0f ) * 0.5f ) + 1.0f ) ;
						}

						thingToScale.Scale = newscale ;

						if( KeyboardStatus.IsKeyDown( Keys.LeftControl ) )
						{
							Vector2 scale ;
							scale.X = (float) Math.Round( thingToScale.Scale.X * 10 ) / 10 ;
							scale.Y = (float) Math.Round( thingToScale.Scale.Y * 10 ) / 10 ;
							thingToScale.Scale = scale ;
						}
						i++ ;
					} ) ;

			IoC.Model.NotifyChanged( allSelectedEditors ) ;

			if( MouseStatus.IsNewRightMouseButtonRelease( ) || KeyboardStatus.IsNewKeyRelease( Keys.D3 ) )
			{
				_editor.SetModeToIdle(  );

				ObjectFactory.GetInstance<IMemento>().EndCommand( ) ;
			}

			return ;
		}

		IEnumerable<ItemEditor> selectedEditors( )
		{
			return getLevel( ).SelectedEditors ;
		}

		Level getLevel( )
		{
			return _model.Level ;
		}
	}
}