using System.IO.Ports;

namespace Service {
  public class SerialPortConvert {
    public static string ParityToString(Parity value) {
      switch (value) {
        case Parity.Even:
          return "Четн.";
        case Parity.Odd:
          return "Нечетн.";
        case Parity.Mark:
          return "Mark";
        case Parity.Space:
          return "Space";
        default:
          return "Нет";
      }
    }

    public static Parity StringToParity(string value) {
      switch (value) {
        case "Четн.":
          return Parity.Even;
        case "Нечетн.":
          return Parity.Odd;
        case "Mark":
          return Parity.Mark;
        case "Space":
          return Parity.Space;
        default:
          return Parity.None;
      }
    }

    public static string StopBitsToString(StopBits value) {
      switch (value) {
        case StopBits.One:
          return "1";
        case StopBits.OnePointFive:
          return "1.5";
        default:
          return "2";
      }
    }

    public static StopBits StringToStopBits(string value) {
      switch (value) {
        case "1":
          return StopBits.One;
        case "1.5":
          return StopBits.OnePointFive;
        default:
          return StopBits.Two;
      }
    }

    public static string HandshakeToString(Handshake value) {
      switch (value) {
        case Handshake.None:
          return "Нет";
        case Handshake.XOnXOff:
          return "XOnXOff";
        default:
          return "Аппаратное";
      }
    }

    public static Handshake StringToHandshake(string value) {
      switch (value) {
        case "Нет":
          return Handshake.None;
        case "XOnXOff":
          return Handshake.XOnXOff;
        default:
          return Handshake.RequestToSend;
      }
    }
  }
}
