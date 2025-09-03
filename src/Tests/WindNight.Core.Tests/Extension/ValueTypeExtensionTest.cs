using System;
using Newtonsoft.Json.Extension;
using WindNight.Core.Extension;
using Xunit;
using Xunit.Abstractions;

namespace WindNight.Core.Tests.Extension
{
    public class ValueTypeExtensionTest : TestBase
    {
        public ValueTypeExtensionTest(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Theory(DisplayName = "DecimalCeilingTest")]
        [InlineData(1.55, 2)]
        [InlineData(-1.45, -1)]
        [InlineData(10.2, 11)]
        public void DecimalCeilingTest(decimal data, int expectData)
        {
            var rlt = data.Ceiling();
            Assert.True(expectData == rlt, $"Ceiling({data})  =>{rlt} !=expectData({expectData})");
            Output($"decimal.Ceiling({data})  =>{rlt},expected is {expectData}");
        }

        [Theory(DisplayName = "DecimalRoundTest")]
        [InlineData(1.55, 2)]
        [InlineData(-1.45, -1)]
        [InlineData(10.2, 10)]
        public void DecimalRoundTest(decimal data, int expectData)
        {
            var rlt = data.Round(0);
            Assert.True(expectData == rlt, $"Round({data})  =>{rlt} !=expectData({expectData})");
            Output($"decimal.Round({data})  =>{rlt},expected is {expectData}");
        }

        [Theory(DisplayName = "DecimalFloorTest")]
        [InlineData(1.55, 1)]
        [InlineData(-1.45, -2)]
        [InlineData(10.2, 10)]
        public void DecimalFloorTest(decimal data, int expectData)
        {
            var rlt = data.Floor();
            Assert.True(expectData == rlt, $"Floor({data})  =>{rlt} !=expectData({expectData})");
            Output($"decimal.Floor({data})  =>{rlt},expected is {expectData}");
        }


        [Theory(DisplayName = "DecimalTruncateTest")]
        [InlineData(1.55, 1)]
        [InlineData(-1.45, -1)]
        [InlineData(10.2, 10)]
        public void DecimalTruncateTest(decimal data, int expectData)
        {
            var rlt = data.Truncate();
            Assert.True(expectData == rlt, $"Truncate({data})  =>{rlt} !=expectData({expectData})");
            Output($"decimal.Truncate({data})  =>{rlt},expected is {expectData}");
        }


        [Theory(DisplayName = "DoubleCeilingTest")]
        [InlineData(1.55, 2)]
        [InlineData(-1.45, -1)]
        [InlineData(10.2, 11)]
        public void DoubleCeilingTest(double data, int expectData)
        {
            var rlt = data.Ceiling();
            Assert.True(expectData == rlt, $"Ceiling({data})  =>{rlt} !=expectData({expectData})");
            Output($"double.Ceiling({data})  =>{rlt},expected is {expectData}");
        }

        [Theory(DisplayName = "DoubleRoundTest")]
        [InlineData(1.55, 2)]
        [InlineData(-1.45, -1)]
        [InlineData(10.2, 10)]
        public void DoubleRoundTest(double data, int expectData)
        {
            var rlt = data.Round(0);
            Assert.True(expectData == rlt, $"Round({data})  =>{rlt} !=expectData({expectData})");
            Output($"double.Round({data})  =>{rlt},expected is {expectData}");
        }

        [Theory(DisplayName = "DoubleFloorTest")]
        [InlineData(1.55, 1)]
        [InlineData(-1.45, -2)]
        [InlineData(10.2, 10)]
        public void DoubleFloorTest(double data, int expectData)
        {
            var rlt = data.Floor();
            Assert.True(expectData == rlt, $"Floor({data})  =>{rlt} !=expectData({expectData})");
            Output($"double.Floor({data})  =>{rlt},expected is {expectData}");
        }

        [Theory(DisplayName = "DoubleTruncateTest")]
        [InlineData(1.55, 1)]
        [InlineData(-1.45, -1)]
        [InlineData(10.2, 10)]
        public void DoubleTruncateTest(double data, int expectData)
        {
            var rlt = data.Truncate();
            Assert.True(expectData == rlt, $"Truncate({data})  =>{rlt} !=expectData({expectData})");
            Output($"double.Truncate({data})  =>{rlt},expected is {expectData}");
        }

        [Theory(DisplayName = "CalcRoomNo1")]
        [InlineData("101", 1, 1)]
        [InlineData("1001", 10, 1)]
        [InlineData("1010", 10, 10)]
        public void CalcRoomNo1(string roomNo, int expectFloor, int expectNo)
        {
            var calcRes = RoomNoParser.ParseRoomNo(roomNo);
            Assert.True(calcRes.floor == expectFloor,
                $"calcFloor({roomNo})  =>{calcRes.floor} !=expectData({expectFloor})");
            Assert.True(calcRes.roomNumber == expectNo,
                $"calcFloor({roomNo})  =>{calcRes.roomNumber} !=expectData({expectNo})");
        }

        [Theory(DisplayName = "CalcRoomNo2")]
        [InlineData("101", 201, 0, 0, 102)]
        [InlineData("1001", 1101, 901, 0, 1002)]
        [InlineData("502", 602, 402, 501, 503)]
        [InlineData("704", 804, 604, 703, 705)]
        [InlineData("1", 0, 0, 0, 0)]
        [InlineData("11", 0, 0, 0, 0)]
        [InlineData("11111", 0, 0, 0, 0)]
        public void CalcRoomNo2(string roomNo, int expecta, int expectu, int expectl, int expectr)
        {
            var calcA = RoomNoParser.CalcAboveRoomNo(roomNo);
            var calcU = RoomNoParser.CalcUnderRoomNo(roomNo);
            var calcL = RoomNoParser.CalcLeftRoomNo(roomNo);
            var calcR = RoomNoParser.CalcRightRoomNo(roomNo);

            Output($"CalcAboveRoomNo({roomNo})  =>{calcA},expected is {expecta}");
            Output($"CalcUnderRoomNo({roomNo})  =>{calcU},expected is {expectu}");
            Output($"CalcLeftRoomNo({roomNo})  =>{calcL},expected is {expectl}");
            Output($"CalcRightRoomNo({roomNo})  =>{calcR},expected is {expectr}");


            Assert.True(calcA == expecta, $"CalcAboveRoomNo({roomNo})  =>{calcA} !=expectData({expecta})");
            Assert.True(calcU == expectu, $"CalcUnderRoomNo({roomNo})  =>{calcU} !=expectData({expectu})");
            Assert.True(calcL == expectl, $"CalcLeftRoomNo({roomNo})  =>{calcL} !=expectData({expectl})");
            Assert.True(calcR == expectr, $"CalcRightRoomNo({roomNo})  =>{calcR} !=expectData({expectr})");
        }

        [Theory(DisplayName = "CalcRoomNo3")]
        [InlineData("101", 201, 0, 0, 102)]
        [InlineData("1001", 1101, 901, 0, 1002)]
        [InlineData("502", 602, 402, 501, 503)]
        [InlineData("704", 804, 604, 703, 705)]
        [InlineData("1", 0, 0, 0, 0)]
        [InlineData("11", 0, 0, 0, 0)]
        [InlineData("11111", 0, 0, 0, 0)]
        public void CalcRoomNo3(string roomNo, int expecta, int expectu, int expectl, int expectr)
        {
            var calcModel = RoomModelHelper.ParserRoomModel(roomNo);
            var calcA = calcModel.AboveRoomModel.RoomNo; // RoomNoParser.CalcAboveRoomNo(roomNo);
            var calcU = calcModel.UnderRoomModel.RoomNo; // RoomNoParser.CalcUnderRoomNo(roomNo);
            var calcL = calcModel.LeftRoomModel.RoomNo; //RoomNoParser.CalcLeftRoomNo(roomNo);
            var calcR = calcModel.RightRoomModel.RoomNo; // RoomNoParser.CalcRightRoomNo(roomNo);

            Output($"ParserRoomModel({roomNo})  =>{calcModel.ToJsonStr()}");
            Output($"AboveRoomModel({roomNo})  =>{calcA},expected is {expecta}");
            Output($"UnderRoomModel({roomNo})  =>{calcU},expected is {expectu}");
            Output($"LeftRoomModel({roomNo})  =>{calcL},expected is {expectl}");
            Output($"RightRoomModel({roomNo})  =>{calcR},expected is {expectr}");


            Assert.True(calcA == expecta, $"AboveRoomModel({roomNo})  =>{calcA} !=expectData({expecta})");
            Assert.True(calcU == expectu, $"UnderRoomModel({roomNo})  =>{calcU} !=expectData({expectu})");
            Assert.True(calcL == expectl, $"LeftRoomModel({roomNo})  =>{calcL} !=expectData({expectl})");
            Assert.True(calcR == expectr, $"RightRoomModel({roomNo})  =>{calcR} !=expectData({expectr})");
        }


        [Theory(DisplayName = "CalcRoomNo4")]
        [InlineData(101, 201, 0, 0, 102)]
        [InlineData(1001, 1101, 901, 0, 1002)]
        [InlineData(502, 602, 402, 501, 503)]
        [InlineData(704, 804, 604, 703, 705)]
        [InlineData(1, 0, 0, 0, 0)]
        [InlineData(11, 0, 0, 0, 0)]
        [InlineData(11111, 0, 0, 0, 0)]
        public void CalcRoomNo4(int roomNo, int expecta, int expectu, int expectl, int expectr)
        {
            var calcModel = RoomModelHelper.ParserRoomModel(roomNo);
            var calcA = calcModel.AboveRoomModel.RoomNo; // RoomNoParser.CalcAboveRoomNo(roomNo);
            var calcU = calcModel.UnderRoomModel.RoomNo; // RoomNoParser.CalcUnderRoomNo(roomNo);
            var calcL = calcModel.LeftRoomModel.RoomNo; //RoomNoParser.CalcLeftRoomNo(roomNo);
            var calcR = calcModel.RightRoomModel.RoomNo; // RoomNoParser.CalcRightRoomNo(roomNo);

            Output($"ParserRoomModel({roomNo})  =>{calcModel.ToJsonStr()}");
            Output($"AboveRoomModel({roomNo})  =>{calcA},expected is {expecta}");
            Output($"UnderRoomModel({roomNo})  =>{calcU},expected is {expectu}");
            Output($"LeftRoomModel({roomNo})  =>{calcL},expected is {expectl}");
            Output($"RightRoomModel({roomNo})  =>{calcR},expected is {expectr}");


            Assert.True(calcA == expecta, $"AboveRoomModel({roomNo})  =>{calcA} !=expectData({expecta})");
            Assert.True(calcU == expectu, $"UnderRoomModel({roomNo})  =>{calcU} !=expectData({expectu})");
            Assert.True(calcL == expectl, $"LeftRoomModel({roomNo})  =>{calcL} !=expectData({expectl})");
            Assert.True(calcR == expectr, $"RightRoomModel({roomNo})  =>{calcR} !=expectData({expectr})");
        }
    }

    public static class RoomNoParser
    {
        // 解析房号为楼层和房间序号
        public static (int floor, int roomNumber) ParseRoomNo(string roomNumber)
        {
            var totalLength = roomNumber.Length;
            var floor = roomNumber.Substring(0, totalLength - 2)
                .ToInt(-99); // int.Parse(roomNumber.Substring(0, totalLength - 2));
            var roomNumberCode =
                roomNumber.Substring(totalLength - 2).ToInt(-99); // int.Parse(roomNumber.Substring(totalLength - 2));
            return (floor, roomNumberCode);
        }

        // 确保房号始终以两位数表示房间序号
        public static string PadRoomNo(string roomNumber)
        {
            var totalLength = roomNumber.Length;
            // TODO 暂时不考虑 百层楼以上的情况
            if (totalLength < 3 || totalLength - 2 > 2) // 检查房号长度是否合理
            {
                return "";
                // throw new ArgumentException("Invalid room number format.");
            }

            return roomNumber;
        }

        // 计算楼上的房号
        public static int CalcAboveRoomNo(string roomNumber)
        {
            roomNumber = PadRoomNo(roomNumber);
            if (roomNumber.IsNullOrEmpty())
            {
                return 0;
            }

            var (floor, roomNumberCode) = ParseRoomNo(roomNumber);
            if (floor <= 0 || roomNumberCode <= 0)
            {
                return 0;
            }

            var roomNo = $"{floor + 1}{roomNumberCode:D2}"; //(floor + 1).ToString() + roomNumberCode.ToString("D2");

            return roomNo.ToInt(-1);
        }

        // 计算楼下的房号
        public static int CalcUnderRoomNo(string roomNumber)
        {
            roomNumber = PadRoomNo(roomNumber);
            if (roomNumber.IsNullOrEmpty())
            {
                return 0;
            }

            var (floor, roomNumberCode) = ParseRoomNo(roomNumber);
            if (floor <= 1 || roomNumberCode <= 0)
            {
                return 0;
            }

            var roomNo = $"{floor - 1}{roomNumberCode:D2}"; // (floor - 1).ToString() + roomNumberCode.ToString("D2");
            return roomNo.ToInt(-1);
        }

        // 计算楼左边的房号
        public static int CalcLeftRoomNo(string roomNumber)
        {
            roomNumber = PadRoomNo(roomNumber);
            if (roomNumber.IsNullOrEmpty())
            {
                return 0;
            }

            var (floor, roomNumberCode) = ParseRoomNo(roomNumber);
            if (floor <= 0 || roomNumberCode <= 1)
            {
                return 0;
            }

            if (roomNumberCode > 0) // 避免房间序号为负数
            {
                //   var roomNo = floor.ToString() + (roomNumberCode - 1).ToString("D2");
                var roomNo =
                    $"{floor}{roomNumberCode - 1:D2}"; // floor.ToString() + (roomNumberCode - 1).ToString("D2");
                return roomNo.ToInt(-1);
            }

            return 0;
            // throw new InvalidOperationException("Room number cannot be on the left side.");
        }

        // 计算楼右边的房号
        public static int CalcRightRoomNo(string roomNumber)
        {
            roomNumber = PadRoomNo(roomNumber);
            if (roomNumber.IsNullOrEmpty())
            {
                return 0;
            }

            var (floor, roomNumberCode) = ParseRoomNo(roomNumber);
            if (floor <= 0 || roomNumberCode <= 0)
            {
                return 0;
            }

            var roomNo = $"{floor}{roomNumberCode + 1:D2}"; // floor.ToString() + (roomNumberCode + 1).ToString("D2");
            return roomNo.ToInt(-1);
        }
    }


    public interface IRoomModel
    {
        int RoomNo { get; set; }

        /// <summary> 楼上房间 房号 </summary>
        IRoomModel AboveRoomModel { get; set; }

        /// <summary> 楼下房间 房号</summary>
        IRoomModel UnderRoomModel { get; set; }

        /// <summary> 左边房间 房号 </summary>
        IRoomModel LeftRoomModel { get; set; }

        /// <summary> 右边房间 房号</summary>
        IRoomModel RightRoomModel { get; set; }
    }

    public class RoomModel : IRoomModel
    {
        public RoomModel()
        {
        }

        public RoomModel(string roomNo)
        {
            RoomNo = roomNo.ToInt();
            if (RoomNo <= 0)
            {
                RoomNo = 0;
            }
        }

        public RoomModel(int roomNo)
        {
            if (roomNo > 0)
            {
                RoomNo = roomNo;
            }
            else
            {
                RoomNo = 0;
            }
        }

        public int RoomNo { get; set; }

        /// <summary> 楼上房间 房号 </summary>
        public IRoomModel AboveRoomModel { get; set; }

        /// <summary> 楼下房间 房号</summary>
        public IRoomModel UnderRoomModel { get; set; }

        /// <summary> 左边房间 房号 </summary>
        public IRoomModel LeftRoomModel { get; set; }

        /// <summary> 右边房间 房号</summary>
        public IRoomModel RightRoomModel { get; set; }
    }

    public static class RoomModelHelper
    {
        public static IRoomModel ParserRoomModel(int roomNo)
        {
            return ParserRoomModel(roomNo.ToString());
        }

        public static IRoomModel ParserRoomModel(string roomNo)
        {
            var model = new RoomModel(roomNo);
            var calcA = RoomNoParser.CalcAboveRoomNo(roomNo);
            var calcU = RoomNoParser.CalcUnderRoomNo(roomNo);
            var calcL = RoomNoParser.CalcLeftRoomNo(roomNo);
            var calcR = RoomNoParser.CalcRightRoomNo(roomNo);

            model.AboveRoomModel = new RoomModel(calcA);
            model.UnderRoomModel = new RoomModel(calcU);
            model.LeftRoomModel = new RoomModel(calcL);
            model.RightRoomModel = new RoomModel(calcR);
            return model;
        }
    }
}
