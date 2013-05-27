using System;
using System.Collections.Generic;

namespace GTZ.Zertz {
	
	public class ZertzMoveCreator : IZertzTriggerListener {
		
		private ZertzGame zertzGame;
		private ZertzBoard zertzBoard;
		private ZertzMoveState state;
		private readonly List<IZertzActionHandler> handlers = new List<IZertzActionHandler>();
		
		public ZertzMoveCreator (ZertzGame game) {
			this.zertzGame = game;
			this.zertzBoard = this.zertzGame.Board;
			this.state = new StartZertzMoveState(this);
		}
		
		private void setState (ZertzMoveState newState) {
			this.state = newState;
			if(this.state is FinishZertzMoveState) {
				this.zertzGame.FinishTurn();
				//EVENTS
				this.perform_ChangeState(0x00);
				this.state = new StartZertzMoveState(this);
			}
		}
		public void TakeBall (ZertzBallType ball) {
			this.setState(state.TakeBall(ball));
		}
		public void SelectPiece (HexLocation location) {
			this.setState(state.SelectPiece(location));
		}
		public void CaptureLand (HexLocation location) {
			this.setState(state.CaptureLand(location));
		}
		public void EndTurn () {
			this.setState(state.EndTurn());
		}
		#region PERFORMERS
		private void perform_PutBall (ZertzBallContainerType containertype, ZertzBallType type, HexLocation location) {
			lock(this.handlers) {
				foreach(IZertzActionHandler izah in this.handlers) {
					izah.PutBall(containertype,type,location);
				}
			}
		}
		internal void perform_ChangePlayer (int newPlayer) {
			lock(this.handlers) {
				foreach(IZertzActionHandler izah in this.handlers) {
					izah.ChangePlayer(newPlayer);
				}
			}
		}
		private void perform_RemovePiece (HexLocation loc) {
			lock(this.handlers) {
				foreach(IZertzActionHandler izah in this.handlers) {
					izah.RemovePiece(loc);
				}
			}
		}
		private void perform_ChangeState (int newState) {
			lock(this.handlers) {
				foreach(IZertzActionHandler izah in this.handlers) {
					izah.ChangeMoveState(newState);
				}
			}
		}
		private void perform_PerformHop (HexLocation fr, HexLocation to) {
			lock(this.handlers) {
				foreach(IZertzActionHandler izah in this.handlers) {
					izah.PerformHop(fr,to);
				}
			}
		}
		private void perform_CaptureBall (HexLocation loc, ZertzBallContainerType zbct) {
			lock(this.handlers) {
				foreach(IZertzActionHandler izah in this.handlers) {
					izah.CaptureBall(loc,zbct);
				}
			}
		}
		#endregion
		public void RegisterActionHandler (IZertzActionHandler handler) {
			lock(this.handlers) {
				this.handlers.Add(handler);
			}
		}
		public void UnregisterActionHandler (IZertzActionHandler handler) {
			lock(this.handlers) {
				this.handlers.Remove(handler);
			}
		}
		
		#region STATE_PATTERN_CLASSES
		private abstract class ZertzMoveState {
			
			private ZertzMoveCreator creator;
			
			protected ZertzBoard Board {
				get {
					return this.creator.zertzBoard;
				}
			}
			protected ZertzGame Game {
				get {
					return this.Creator.zertzGame;
				}
			}
			
			protected ZertzMoveCreator Creator {
				get {
					return this.creator;
				}
			}
			
			protected ZertzMoveState (ZertzMoveCreator creator) {
				this.creator = creator;
			}
			
			protected ZertzMoveState NotPossible () {
				throw new InvalidZertzActionException("Unable to do this type of action at the moment!");
				return null;
			}
			public virtual ZertzMoveState TakeBall (ZertzBallType ball) {//TB
				return NotPossible();
			}
			public virtual ZertzMoveState SelectPiece (HexLocation location) {//SP
				return NotPossible();
			}
			public virtual ZertzMoveState CaptureLand (HexLocation location) {//CL
				return NotPossible();
			}
			public virtual ZertzMoveState EndTurn () {//ET
				return NotPossible();
			}
			
		}
		private class StartZertzMoveState : ZertzMoveState {
			
			public StartZertzMoveState (ZertzMoveCreator creator) : base(creator) {}
			
			public override ZertzMoveState SelectPiece (HexLocation location) {
				//GUARDS
				if(!this.Board.CanHop(location)) {
					throw new InvalidZertzActionException("Unable to do this action: the ball can't hop!");
				}
				//ACTIONS
				//EVENTS
				this.Creator.perform_ChangeState(0x02);
				//NEXT_STATE
				return new Capture1ZertzMoveState(this.Creator,location);
			}
			public override ZertzMoveState TakeBall (ZertzBallType ball) {
				//GUARD
				if(this.Board.CanHop()) {
					throw new InvalidZertzActionException("Unable to do this action: capture move possible!");
				}
				if(!this.Game.PlayerPoolsContains(ball)) {
					throw new InvalidZertzActionException("Unable to do this action: player doesn't have this type of ball available!");
				}
				if(!this.Board.HasVacant()) {
					throw new InvalidZertzActionException("Unable to do this action: no vacant piece available!");
				}
				ZertzBallContainerType source = this.Game.SourceType;
				//ACTIONS
				this.Game.DecreasePlayerPool(ball);
				//EVENTS
				this.Creator.perform_ChangeState(0x01);
				//NEXT_STATE
				return new PlacRem1ZertzMoveState(this.Creator,ball,source);
			}
			
		}
		
		private class PlacRem1ZertzMoveState : ZertzMoveState {
			
			private readonly ZertzBallType ball;
			private readonly ZertzBallContainerType source;
			
			public ZertzBallType Ball {
				get {
					return this.ball;
				}
			}
			
			public PlacRem1ZertzMoveState (ZertzMoveCreator creator, ZertzBallType ball, ZertzBallContainerType source) : base(creator) {
				this.ball = ball;
				this.source = source;
			}
			
			public override ZertzMoveState SelectPiece (HexLocation location) {
				//GUARDS
				if(!this.Board.IsVacant(location)) {
					throw new InvalidZertzActionException("Unable to do this action: ball must be put on a vacant piece!");
				}
				//ACTIONS
				this.Board.PutZertzBall(location,this.Ball);
				//EVENT
				this.Creator.perform_PutBall(this.source,this.ball,location);
				this.Creator.perform_ChangeState(0x03);
				//NEXT_STATE
				return new PlacRem2ZertzMoveState(this.Creator);
			}
			
		}
		
		private class PlacRem2ZertzMoveState : ZertzMoveState {
			
			public PlacRem2ZertzMoveState (ZertzMoveCreator creator) : base(creator) {}
			
			public override ZertzMoveState SelectPiece (HexLocation location) {
				//GUARDS
				if(!this.Board.IsFree(location)) {
					throw new InvalidZertzActionException("Unable to do this action: This piece can't be removed!");
				}
				//ACTIONS
				this.Board.RemoveZertzPiece(location);
				//EVENT
				this.Creator.perform_RemovePiece(location);
				this.Creator.perform_ChangeState(0x05);
				//NEXT_STATE
				return new CaptureLandZertzMoveState(this.Creator);
			}
			public override ZertzMoveState CaptureLand (HexLocation location) {
				//GUARDS
				if(this.Board.HasFree()) {
					throw new InvalidZertzActionException("Unable to do this action: An free piece must first be removed!");
				}
				if(!this.Board.CanCapture(location)) {
					throw new InvalidZertzActionException("Unable to do this action: This land cannot be captured!");
				}
				//ACTIONS
				this.Game.PutToPool(this.Board.CaptureLandPieces(location));
				//EVENTS
				this.Creator.perform_ChangeState(0x05);
				//TODO: insert event
				//NEXT_STATE
				return new CaptureLandZertzMoveState(this.Creator);
			}
			public override ZertzMoveState EndTurn () {
				//GUARDS
				if(this.Board.HasFree()) {
					throw new InvalidZertzActionException("Unable to do this action: at least one piece can be removed!");
				}
				//ACTIONS
				//EVENTS
				this.Creator.perform_ChangeState(0x06);
				//NEXT_STATE
				return new FinishZertzMoveState(this.Creator);
			}
			
		}
		
		private abstract class CaptureZertzMoveState : ZertzMoveState {
			
			private readonly HexLocation hopLocation;
			
			protected HexLocation HopLocation {
				get {
					return this.hopLocation;
				}
			}
			
			public CaptureZertzMoveState (ZertzMoveCreator creator, HexLocation hopLocation) : base(creator) {
				this.hopLocation = hopLocation;
			}
			
			public override ZertzMoveState SelectPiece (HexLocation location) {
				HexLocation dir = !(location-this.HopLocation);
				//GUARDS
				if(dir == HexLocation.Invalid) {
					throw new InvalidZertzActionException("Unable to perform this action: No direction specified!");
				}
				else if(!this.Board.CanHop(this.HopLocation,dir.HexDirection)) {
					throw new InvalidZertzActionException("Unable to perform this action: Can't hop in the given direction!");
				}
				//ACTIONS
				this.Game.PutToPool(this.Board.DoHopMove(this.HopLocation,dir.HexDirection));
				//EVENTS
				this.Creator.perform_ChangeState(0x04);
				this.Creator.perform_PerformHop(hopLocation,location);
				//TODO: insert capture action
				//NEXT_STATE
				HexLocation newl = this.HopLocation+(dir<<0x01);
				return new Capture2ZertzMoveState(this.Creator,newl);
			}
			
		}
		
		private class Capture1ZertzMoveState : CaptureZertzMoveState {
			
			public Capture1ZertzMoveState (ZertzMoveCreator creator, HexLocation hopLocation) : base(creator,hopLocation) {}
			
		}
		
		private class Capture2ZertzMoveState : CaptureZertzMoveState {
			
			public Capture2ZertzMoveState (ZertzMoveCreator creator, HexLocation hopLocation) : base(creator,hopLocation) {}
			
			public override ZertzMoveState CaptureLand (HexLocation location) {
				//GUARDS
				if(this.Board.CanHop(this.HopLocation)) {
					throw new InvalidZertzActionException("Unable to do this action: ball can perform at least one extra hop!");
				}
				if(!this.Board.CanCapture(location)) {
					throw new InvalidZertzActionException("Unable to do this action: This land cannot be captured!");
				}
				//ACTIONS
				this.Game.PutToPool(this.Board.CaptureLandPieces(location));
				//EVENTS
				this.Creator.perform_ChangeState(0x05);
				//NEXT_STATE
				return new CaptureLandZertzMoveState(this.Creator);
			}
			public override ZertzMoveState EndTurn () {
				//GUARDS
				if(this.Board.CanHop(this.HopLocation)) {
					throw new InvalidZertzActionException("Unable to do this action: ball can perform at least one extra hop!");
				}
				//ACTIONS
				//EVENTS
				this.Creator.perform_ChangeState(0x06);
				//NEXT_STATE
				return new FinishZertzMoveState(this.Creator);
			}
			
		}
		
		private class CaptureLandZertzMoveState : ZertzMoveState {
			
			public CaptureLandZertzMoveState (ZertzMoveCreator creator) : base(creator) {}
			
			public override ZertzMoveState CaptureLand (HexLocation location) {
				//GUARDS
				if(!this.Board.CanCapture(location)) {
					throw new InvalidZertzActionException("Unable to do this action: This land cannot be captured!");
				}
				//ACTIONS
				this.Game.PutToPool(this.Board.CaptureLandPieces(location));
				//EVENTS
				this.Creator.perform_ChangeState(0x05);
				//NEXT_STATE
				return new CaptureLandZertzMoveState(this.Creator);
			}
			public override ZertzMoveState EndTurn () {
				//GUARDS
				//ACTIONS
				//EVENTS
				this.Creator.perform_ChangeState(0x06);
				//NEXT_STATE
				return new FinishZertzMoveState(this.Creator);
			}
			
		}
		
		private class FinishZertzMoveState : ZertzMoveState {
			
			public FinishZertzMoveState (ZertzMoveCreator creator) : base(creator) {}
			
		}
		#endregion
		
	}
}

