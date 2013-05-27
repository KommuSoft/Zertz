using System;

namespace Zertz.Zertz {
	
	public class ZertzGame : IGame, IZertzTriggerListener {
		
		private readonly IPlayer[] players;
		private int turn;
		private readonly ZertzBoard board;
		private readonly ZertzBallContainer commonPool = ZertzBallContainer.Offset();
		private readonly ZertzBallContainer[] playersPools;
		private ZertzMoveCreator zmc;
		
		public IPlayer[] Players {
			get {
				return this.players;
			}
		}
		public ZertzBoard Board {
			get {
				return this.board;
			}
		}
		public ZertzBallContainerType SourceType {
			get {
				if(this.commonPool != ZertzBallContainer.Empty()) {
					return ZertzBallContainerType.Common;
				}
				else {
					return (ZertzBallContainerType) (this.turn+0x01);
				}
			}
		}
		public ZertzBallContainerType DestinationType {
			get {
				return (ZertzBallContainerType) (this.turn+0x01);
			}
		}
		
		public ZertzGame (out HexLocation[] hls) {
			//this.players = new IPlayer[] {playerA,playerB};
			this.board = ZertzBoard.OffsetBoard(out hls);
			this.playersPools = new ZertzBallContainer[] {ZertzBallContainer.Empty(),ZertzBallContainer.Empty()};
			this.turn = 0x00;
			this.zmc = new ZertzMoveCreator(this);
		}
		
		private ZertzBallContainer getCurrentPlayerSourcePool () {
			if(this.commonPool != ZertzBallContainer.Empty()) {
				return this.commonPool;
			}
			else {
				return this.playersPools[this.turn];
			}
		}
		private ZertzBallContainer getCurrentPlayerDestinationPool () {
			return this.playersPools[this.turn];
		}
		
		public void RegisterActionHandler (IZertzActionHandler handler) {
			this.zmc.RegisterActionHandler(handler);
		}
		public void UnregisterActionHandler (IZertzActionHandler handler) {
			this.zmc.UnregisterActionHandler(handler);
		}
		#region STATE_CONDITIONS
		public bool PlayerPoolsContains (ZertzBallType type) {//BHP
			return (this.getCurrentPlayerSourcePool()[type] > 0x00);
		}
		#endregion
		#region ACTION_COMMANDS
		public void DecreasePlayerPool (ZertzBallType type) {//DPP
			this.getCurrentPlayerSourcePool()[type]--;
		}
		public void PutToPool (ZertzBallType type) {//PTP
			this.getCurrentPlayerDestinationPool().Add(type);
		}
		public void PutToPool (ZertzBallContainer container) {//PTP
			this.getCurrentPlayerDestinationPool().Add(container);
		}
		#endregion
		#region TRIGGERS
		public void SelectPiece (HexLocation location) {
			this.zmc.SelectPiece(location);
		}
		public void TakeBall (ZertzBallType ball) {
			this.zmc.TakeBall(ball);
		}
		public void CaptureLand (HexLocation location) {
			this.zmc.CaptureLand(location);
		}
		public void EndTurn () {
			this.zmc.EndTurn();
		}
		#endregion
		public void FinishTurn () {
			//TODO: finish turn (check victorious)
			this.turn = 0x01-this.turn;
			//EVENT
			this.zmc.perform_ChangePlayer(this.turn);
			//TODO: finish turn (check victorious)
		}
		
	}
	
}