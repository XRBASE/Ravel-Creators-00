using System;
using System.Collections.Generic;

namespace Base.Ravel.LogList
{
	public class LogList
	{
		public Log this[int i] {
			get { return logs[i]; }
		}

		public int Count {
			get { return logs.Count; }
		}

		private List<Log> logs;

		public LogList() {
			logs = new List<Log>();
		}
		
		public LogList(int capacity) {
			logs = new List<Log>(capacity);
		}

		public List<Log> GetOfType(Log.LogType type) {
			List<Log> data = new List<Log>();
			for (int i = 0; i < Count; i++) {
				if (type.HasFlag(this[i].type)) {
					data.Add(this[i]);
				}
			}

			return data;
		}

		public void AddLog(string msg, Log.LogType t, bool debug = true) {
			AddLog(new Log(msg, t), debug);
		}
		
		public void AddLog(Log l, bool debug = true) {
			logs.Add(l);
			if (debug) {
				l.Debug();
			}
		}

		public void Clear() {
			logs.Clear();
		}
	}

	public struct Log
	{
		public string msg;
		public LogType type;

		public Log(string msg, LogType type) {
			this.msg = msg;
			this.type = type;
		}

		public void Debug() {
			switch (type) {
				case LogType.Log:
					UnityEngine.Debug.Log(msg);
					break;
				case LogType.Warning:
					UnityEngine.Debug.LogWarning(msg);
					break;
				case LogType.Error:
					UnityEngine.Debug.LogError(msg);
					break;
				default:
					throw new Exception($"Type mismatch for log ({msg}) of type ({type})");
			}
		}
		
		[Flags]
		public enum LogType
		{
			Log = 1<<0,
			Warning = 1<<1,
			Error = 1<<2,
		}
	}
}