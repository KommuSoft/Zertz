using System;
using System.Collections.Generic;

namespace Zertz {
	
	public class Caretaker<T> where T : ICloneable {
		
		private readonly List<Entry<T>> data = new List<Entry<T>>();
		
		public Entry<T> this [int i] {
			get {
				if(i >= 0x00) {
					return data[Math.Min(i, data.Count-0x01)];
				}
				else {
					return data[Math.Max(0x00, data.Count+i)];
				}
			}
		}
		
		public Caretaker () {
		}
		
		public void MakeMemento (T obj) {
			data.Add(new Entry<T>(obj));
		}
		
		public class Entry<T> where T : ICloneable {
			
			private readonly DateTime time;
			private readonly T obj;
			
			public DateTime Time {
				get {
					return this.time;
				}
			}
			public T Obj {
				get {
					return this.obj;
				}
			}
			
			internal Entry (T obj) {
				this.time = DateTime.Now;
				this.obj = (T)obj.Clone();
			}
			
		}
		
	}
	
}