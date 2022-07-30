// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=SimpleStateMachineBase_Forms.cs
// company="Marcus Technical Services, Inc.">
// </copyright>
// 
// MIT License
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// *********************************************************************************

// #define FORCE_STATE_CHANGE_ON_MAIN_THREAD

namespace Com.MarcusTS.UI.XamForms.Common.Navigation
{
   using System.Collections.Generic;
   using System.Threading.Tasks;
   using Com.MarcusTS.PlatformIndependentShared.ViewModels;
   using Com.MarcusTS.SharedUtils.Utils;
   using Com.MarcusTS.SmartDI;
   using Com.MarcusTS.UI.XamForms.Common.Utils;

   /// <summary>
   /// Interface IStateMachineBase
   /// </summary>
   public interface ISimpleStateMachine_Forms
   {
      /// <summary>
      /// Gets the default state.
      /// </summary>
      /// <value>The default state.</value>
      string DefaultState { get; }

      /// <summary>
      /// Gets or sets the di container.
      /// </summary>
      /// <value>The di container.</value>
      ISmartDIContainer DIContainer { get; set; }

      /// <summary>
      /// Gets the start up state.
      /// </summary>
      /// <value>The start up state.</value>
      string StartUpState { get; }

      /// <summary>
      /// Goes the state of to application.
      /// </summary>
      /// <param name="newState">The new state.</param>
      /// <param name="forceState">Ignores the comparison with the last app state; this change must occur.</param>
      /// <param name="andRebuildToolbars">if set to <c>true</c> [and rebuild toolbars].</param>
      /// <returns>Task.</returns>
      Task GoToAppState
      (
         string newState,
         bool   forceState         = false,
         bool   andRebuildToolbars = false
      );

      /// <summary>
      /// Goes to default state.
      /// </summary>
      /// <param name="forceAppState">if set to <c>true</c> [force application state].</param>
      /// <param name="andRebuildStageToolbars">if set to <c>true</c> [and rebuild stage toolbars].</param>
      /// <returns>Task.</returns>
      Task GoToDefaultState( bool forceAppState = false, bool andRebuildStageToolbars = false );

      /// <summary>
      /// Goes the state of to last application.
      /// </summary>
      /// <returns>Task.</returns>
      Task GoToLastAppState();

      /// <summary>
      /// Goes the state of to start up.
      /// </summary>
      /// <returns>Task.</returns>
      Task GoToStartUpState();
   }

   /// <summary>
   /// Class StateMachineBase. Implements the <see cref="ISimpleStateMachine_Forms" /> Implements the
   /// <see cref="ISimpleStateMachine_Forms" />
   /// </summary>
   /// <seealso cref="ISimpleStateMachine_Forms" />
   /// <seealso cref="ISimpleStateMachine_Forms" />
   public abstract class SimpleStateMachineBase_Forms : PropertyChangedBase_PI, ISimpleStateMachine_Forms
   {
      /// <summary>
      /// The last application state
      /// </summary>
      private string _lastAppState;

      /// <summary>
      /// Gets the default state.
      /// </summary>
      /// <value>The default state.</value>
      public abstract string DefaultState { get; }

      /// <summary>
      /// Gets or sets the di container.
      /// </summary>
      /// <value>The di container.</value>
      public virtual ISmartDIContainer DIContainer { get; set; } = new SmartDIContainer();

      /// <summary>
      /// Gets the start up state.
      /// </summary>
      /// <value>The start up state.</value>
      public abstract string StartUpState { get; }

      /// <summary>
      /// Goes the state of to application.
      /// </summary>
      /// <param name="newState">The new state.</param>
      /// <param name="forceState">Ignores the comparison with the last app state; this change must occur.</param>
      /// <param name="andRebuildToolbars">if set to <c>true</c> [and rebuild toolbars].</param>
      /// <returns>Task.</returns>
      public async Task GoToAppState
      (
         string newState,
         bool   forceState         = false,
         bool   andRebuildToolbars = false
      )
      {
         if ( !forceState && _lastAppState.IsSameAs( newState ) )
         {
            return;
         }

         // Done early to prevent recursion
         _lastAppState = newState;

         await RespondToAppStateChange( newState, andRebuildToolbars ).ConsiderBeginInvokeTaskOnMainThread(
#if FORCE_STATE_CHANGE_ON_MAIN_THREAD
         true
#else

            // ReSharper disable once RedundantArgumentDefaultValue
            false
#endif
         ).AndReturnToCallingContext();
      }

      /// <summary>
      /// Goes to default state.
      /// </summary>
      /// <param name="forceAppState">if set to <c>true</c> [force application state].</param>
      /// <param name="andRebuildStageToolbars">if set to <c>true</c> [and rebuild stage toolbars].</param>
      /// <returns>Task.</returns>
      public async Task GoToDefaultState( bool forceAppState = false, bool andRebuildStageToolbars = false )
      {
         await GoToAppState( DefaultState, forceAppState, andRebuildStageToolbars ).AndReturnToCallingContext();
      }

      /// <summary>
      /// Goes the state of to last application.
      /// </summary>
      /// <returns>Task.</returns>
      public async Task GoToLastAppState()
      {
         await GoToAppState( _lastAppState, true ).AndReturnToCallingContext();
      }

      /// <summary>
      /// Goes the state of to start up.
      /// </summary>
      /// <returns>Task.</returns>
      public async Task GoToStartUpState()
      {
         await GoToAppState( StartUpState, true ).AndReturnToCallingContext();
      }

      /// <summary>
      /// Goes to a new state and then continues digging through nested states until the nested paths have been
      /// exhausted.
      /// </summary>
      /// <param name="nestedPaths">The nested paths.</param>
      /// <returns>Task.</returns>
      public static async Task GoToAppStateWithAdditionalPaths( StateMachineSubPath_Forms[] nestedPaths )
      {
         if ( nestedPaths.IsEmpty() )
         {
            return;
         }

         // Go to the first path
         await nestedPaths[ 0 ].StateMachineForms.GoToAppState( nestedPaths[ 0 ].AppState, true ).AndReturnToCallingContext();

         if ( nestedPaths.Length == 1 )
         {
            return;
         }

         var newlyNestedPaths = new List<StateMachineSubPath_Forms>();
         for ( var pathIdx = 1; pathIdx < nestedPaths.Length; pathIdx++ )
         {
            newlyNestedPaths.Add( new StateMachineSubPath_Forms
                                  {
                                     StateMachineForms = nestedPaths[ pathIdx ].StateMachineForms,
                                     AppState     = nestedPaths[ pathIdx ].AppState,
                                  } );
         }

         var newlyNestedPathArray = newlyNestedPaths.ToArray();

         // Recur until finished
         await GoToAppStateWithAdditionalPaths( newlyNestedPathArray ).AndReturnToCallingContext();
      }

      /// <summary>
      /// Responds to application state change.
      /// </summary>
      /// <param name="newState">The new state.</param>
      /// <param name="andRebuildToolbars">if set to <c>true</c> [and rebuild toolbars].</param>
      /// <returns>Task.</returns>
      protected abstract Task RespondToAppStateChange( string newState, bool andRebuildToolbars = false );
   }
}