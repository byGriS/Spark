using System;

namespace SparkInput {
  public class PipeCounterHelper {
    public int IndexCounter { get; set; }
    public bool IsInitCounterPipe { get; set; }
    public DateTime DateTimeCounterPipe { get; set; }
    public float CountPipe { get; set; }

    private float pipeLengthPre = -1;
    private float pipeLengthStart = -1;
    private bool isPipe = false;
    private bool isMove = false;
    private bool pipeUp = true;
    private DateTime lastMoveTime = DateTime.Now;
    private DateTime resetTime = DateTime.Now;

    public static double dempfer = 0.5;
    public static double lengthFrom = 7;
    public static double lengthTo = 12;
    public static double pauseTime = 40;

    public void CalcPipe(float curLength) {
      if ((pipeLengthPre == -1) && (pipeLengthStart == -1)) {
        pipeLengthPre = curLength;
        pipeLengthStart = curLength;
      }

      if (curLength > pipeLengthStart)
        pipeUp = true;
      else
        pipeUp = false;

      if (Math.Abs(curLength- pipeLengthPre) > dempfer) {

        if (Math.Abs(curLength - pipeLengthStart) >= lengthFrom)
          isPipe = true;

        if (Math.Abs(curLength - pipeLengthStart) >= lengthTo) {
          if (pipeUp) CountPipe++; else CountPipe--;
          pipeLengthStart = curLength;
          isPipe = false;
        }
        lastMoveTime = DateTime.Now;
      }

      /*if (curLength > pipeLengthPre + dempfer) {        
        if (curLength - pipeLengthStart >= lengthTo) {
          CountPipe++;
          pipeLengthStart = curLength;
        }
        if (curLength - pipeLengthStart >= lengthFrom)
          isPipe = true;
        else
          isPipe = false;
        lastMoveTime = DateTime.Now;
      }

      if (curLength < pipeLengthPre - dempfer) {
        if (pipeLengthStart - curLength >= lengthTo) {
          CountPipe--;
          pipeLengthStart = curLength;
        }
        if (pipeLengthStart - curLength >= lengthFrom)
          isPipe = true;
        else
          isPipe = false;
        lastMoveTime = DateTime.Now;
      }*/

      if (Math.Abs(curLength - pipeLengthPre) < dempfer) {      
        double subTime = DateTime.Now.Subtract(lastMoveTime).TotalSeconds;
        if (subTime > pauseTime) {
          if (isPipe) {
            if (pipeUp)
              CountPipe++;
            else
              CountPipe--;
            pipeLengthStart = curLength;
            isPipe = false;
          }
        }
      } else {
        pipeLengthPre = curLength;
      }
      
      /*if (!isMove && curLength > pipeLengthPre + dempfer) {
        isMove = true;
      }
      if (Math.Abs(curLength - pipeLengthPre) < 0.05) {
        isMove = false;
      }

      if (isMove) {
        if (curLength > pipeLengthPre) {
          pipeUp = true;

          if (curLength - pipeLengthStart >= lengthFrom)
            isPipe = true;
          else
            isPipe = false;

          if (curLength - pipeLengthStart >= lengthTo) {
            CountPipe++;
            pipeLengthStart = curLength;
            isPipe = false;
          }
          lastMoveTime = DateTime.Now;
        }
        pipeLengthPre = curLength;
      } else {
        if (isPipe) {
          double subTime = DateTime.Now.Subtract(lastMoveTime).TotalSeconds;
          if (subTime > pauseTime) {
            if (pipeUp)
              CountPipe++;
            else
              CountPipe--;
            pipeLengthStart = curLength;
            isPipe = false;
          }
        }
      }*/


      if (Math.Abs(curLength) < 1) {
        double subTimeReset = DateTime.Now.Subtract(resetTime).TotalSeconds;
        if (subTimeReset > 60) {
          Reset();
        }
      } else {
        resetTime = DateTime.Now;
      }

      return;
    }

    public void Reset() {
      DateTimeCounterPipe = DateTime.Now;
      pipeLengthPre = -1;
      pipeLengthStart = -1;
      isPipe = false;
      isMove = false;
      pipeUp = true;
      CountPipe = 0;
      lastMoveTime = DateTime.Now;
    }
  }
}
