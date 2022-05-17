// *********************************************************************************
// Copyright @2022 Marcus Technical Services, Inc.
// <copyright
// file=StateMachineSubPath_PI.cs
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

namespace Com.MarcusTS.UI.XamForms.Common.Navigation
{
   /// <summary>
   /// Class StateMachineSubPath_Forms.
   /// </summary>
   public class StateMachineSubPath_Forms
   {
      /// <summary>
      /// Gets or sets the state of the application.
      /// </summary>
      /// <value>The state of the application.</value>
      public string AppState { get; set; }

      /// <summary>
      /// Gets or sets the state machine.
      /// </summary>
      /// <value>The state machine.</value>
      public ISimpleStateMachine_Forms StateMachineForms { get; set; }
   }
}