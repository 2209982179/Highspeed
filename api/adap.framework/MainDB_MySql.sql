CREATE TABLE IF NOT EXISTS  TestCase  (
  `TestCaseID` int NOT NULL AUTO_INCREMENT,
  `ParentID` int DEFAULT NULL,
  `TestCaseGUID` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField1` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField2` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `TestCaseName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `TestCaseType` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `TrainType` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `TestCommands` int DEFAULT NULL,
  `TestInitializeParameters` int DEFAULT NULL,
  `TestCheckedParameters` int DEFAULT NULL,
  `TestTrainInformation` int DEFAULT NULL,
  `CreateTime` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ModificationTime` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `Remark` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
  `ExtendedField3` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField4` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField5` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField6` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField7` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField8` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField9` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField10` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  PRIMARY KEY (`TestCaseID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;


CREATE TABLE IF NOT EXISTS  TestStep  (
  `TestCaseID` int NOT NULL,
  `TestStepID` int NOT NULL AUTO_INCREMENT,  
  `ParentID` int NOT NULL,
  `PreviousID` int NOT NULL,
  `TestStepType` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `sendReceiveType` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `TestStepName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `StepDescription` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `StepValue` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ParameterValueType` int DEFAULT NULL,
  `CreateTime` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ModificationTime` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `Execute` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `IfPrint` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `Remark` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
  `ExtendedField1` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField2` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField3` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField4` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField5` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField6` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField7` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField8` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField9` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField10` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  PRIMARY KEY (`TestStepID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;


CREATE TABLE IF NOT EXISTS  TestParameter  (
  `ParameterID` int NOT NULL AUTO_INCREMENT,
  `ParameterKey` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ParameterDescription` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `DefaultValue` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `TestValue` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ParameterValueType` int DEFAULT NULL,
  `Remark` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
  `ExtendedField1` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField2` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField3` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField4` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField5` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField6` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField7` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField8` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField9` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField10` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  PRIMARY KEY (`ParameterID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

CREATE TABLE IF NOT EXISTS  TestRecode  (
  `RecodeID` int NOT NULL AUTO_INCREMENT,
  `RecodeName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `Remark` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
  `ExtendedField1` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField2` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField3` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  PRIMARY KEY (`RecodeID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

CREATE TABLE IF NOT EXISTS  TestTrainInformation  (
  `TrainID` int NOT NULL AUTO_INCREMENT,
  `TrainName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `TrainType` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `FormationType` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `VehicleLoad` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `MaximumOperatingSpeed` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `AverageTravelSpeed` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `AverageDwellTime` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `AverageAcceleration` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `AverageDeceleration` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `TrainImpactRate` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `RequirementsFaultStatus` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `RequirementsRampRescue` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `RequirementsNoiseLevelIn` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `RequirementsNoiseLevelOut` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `Remark` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField1` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField2` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField3` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField4` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField5` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField6` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField7` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField8` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField9` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ExtendedField10` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  PRIMARY KEY (`TrainID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

CREATE TABLE IF NOT EXISTS  CaseAndParameter  (
  `TestCaseID` int NOT NULL,
  `ParameterID` int NOT NULL,
  PRIMARY KEY (`TestCaseID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

CREATE TABLE IF NOT EXISTS  CaseAndRecode  (
  `TestCaseID` int NOT NULL,
  `RecodeID` int NOT NULL,
  PRIMARY KEY (`TestCaseID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

CREATE TABLE IF NOT EXISTS  CaseAndTrain  (
  `TestCaseID` int NOT NULL,
  `TrainID` int NOT NULL,
  PRIMARY KEY (`TestCaseID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

CREATE TABLE IF NOT EXISTS   ProtocolConfig  (
  `Category` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `CategoryRemark` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `Description` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `DescriptionRemark` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `Content` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `ContentRemark` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL,
  `Remark` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT NULL
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

INSERT INTO TestRecode (RecodeName, Remark, ExtendedField1, ExtendedField2, ExtendedField3)
SELECT * FROM (
    SELECT '速度' AS RecodeName, NULL AS Remark, NULL AS ExtendedField1, NULL AS ExtendedField2, NULL AS ExtendedField3
    UNION ALL
    SELECT '网压', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '网流', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '牵引制动级位', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '牵引力需求值', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '牵引力反馈值', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '逆变器输出电压', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '逆变器输出电流', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '电机电流', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '制动初速度', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '实时加速度', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '电制动力需求值', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '电制动力反馈值', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '拖车制动缸压力', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '动车制动缸压力', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '制动距离', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '制动级位', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '制动指令', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '电制动切除', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '主断闭合断开', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '逆变器输入电流', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '逆变器温升', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '电机温升', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '站停时间', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '列车总能耗', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '牵引能耗', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '再生能耗', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '牵引级位', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '加速度', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '压力', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '燃弧', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '激光系统传感器', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '测速雷达', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '视频监控', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '紧急制动指令', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '车辆紧急制动时的速度', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '制动盘温度', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '闸片温度', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '拖车压力', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '每站停站时间', NULL, NULL, NULL, NULL
    UNION ALL
    SELECT '对应车轮温度', NULL, NULL, NULL, NULL
) AS tmp
WHERE NOT EXISTS (SELECT 1 FROM TestRecode);

INSERT INTO TestTrainInformation (TrainName, TrainType, FormationType, VehicleLoad, TestValue,MaximumOperatingSpeed,AverageTravelSpeed,AverageDwellTime,AverageAcceleration,AverageDeceleration,
TrainImpactRate,RequirementsFaultStatus,RequirementsRampRescue,RequirementsNoiseLevelIn,RequirementsNoiseLevelOut,Remark,ExtendedField1,ExtendedField2,ExtendedField3,ExtendedField4,ExtendedField5,ExtendedField6,
ExtendedField7,ExtendedField8,ExtendedField9,ExtendedField10)
SELECT * FROM (
    SELECT 'AW0' AS TrainName, 'AW0' AS TrainType,  '4动2拖，Tc+Mp+M+M+Mp+Tc' AS FormationType,  NULL AS VehicleLoad,  NULL AS TestValue, '80km/h' AS MaximumOperatingSpeed, '≥35km/h（平均站停时间30s）' AS AverageTravelSpeed, '（1）列车从0加速到40km/h≥1.0m/s²；（2）列车从0加速到80km/h≥0.6m/s²。' AS AverageDwellTime, '（1）最大常用制动1.0m/s²；（2）紧急制动≥1.2m/s²；（3）快速制动≥1.2m/s²。' AS AverageAcceleration, 
    '（1）最大常用制动1.0m/s²；（2）紧急制动≥1.2m/s²；（3）快速制动≥1.2m/s²。（1）最大常用制动1.0m/s²；（2）紧急制动≥1.2m/s²；（3）快速制动≥1.2m/s²。' AS AverageDeceleration,
 '≤0.75m/s³（紧急制动除外）' AS TrainImpactRate, '（1）一列6辆编组的列车在超载条件下，当损失1/4牵引动力时，列车仍然可以在用户现场最大坡道上起动，并能以正常运行方式完成当天运行；（2）一列6辆编组的列车在超载条件下，在损失1/2牵引动力时，列车仍然可以在用户现场最大坡道上起动和运行到最近车站的能力，清客后返回车辆段。' AS RequirementsFaultStatus, 
 '（1）一列6辆编组的空载列车应能将另一列停在用户现场最大坡道上的6辆编组超载故障列车牵引至最近的车站（上坡）；（2）一列6辆编组的空载列车应能将另一列停在用户现场最大坡道上的6辆编组故障空载列车救援到车辆段（上坡）。' AS RequirementsRampRescue, '（1）列车静止条件下辅助设备的噪音。列车处于静止状态和自由声场内，所有辅助设备正常运行时，客室内部沿车辆中心线、距离地板面1.5m高处至少测量3个点，测得的噪声水平≤69dB（A）。在空调回风口下方测得的噪声水平≤72dB（A）；（2）列车在地面线路道渣轨道上运行时的噪声。列车以正常方式加速、惰行或制动时，客室内部沿车辆中心线、距离地板面1.5m高处至少测量3个点，测得的噪声水平≤74dB（A），在贯通道附近和空调回风口下方，距离任意墙面不少于0.3m处，测得的噪声水平≤75dB（A）。司机室内噪声≤77dB（A）。列车以不超过80km/h的任意恒定速度（通常列车以60km/h±5%的恒定速度）运行时，车内中心距地板面高1.5m处测得的噪声水平≤73dB（A），司机室内测得的噪声≤74dB（A），恒速运行时间为60s。' AS RequirementsNoiseLevelIn,
 '（1）静止条件下辅助设备的噪音。空载列车在静止状态和自由声场内，所有辅助设备同时运行时，沿水平方向距离走行轨线路中心线7.5m处，在列车任意一侧、列车长度范围内的任意点测得的噪音≤72dB（A）。测试在ISO3095规定的自由区域条件下，列车在露天地面区段进行。（2）列车在地面线路道渣轨道上运行时的噪声。列车以正常方式加速、惰行或减速运行时，沿水平方向距离线路中心线7.5m处测量，车辆发出的噪声≤80dB（A）。车外噪声的测试应根据ISO3095进行。列车以不超过80km/h（通常列车以60km/h±5%的恒定速度）运行时，沿水平方向距离线路中心线7.5m处测量，车辆发出的噪声≤80dB（A）。' AS RequirementsNoiseLevelOut, NULL AS Remark, NULL AS ExtendedField1, NULL AS ExtendedField2, NULL AS ExtendedField3, NULL AS ExtendedField4, NULL AS ExtendedField5, NULL AS ExtendedField6,
 NULL AS ExtendedField7, NULL AS ExtendedField8, NULL AS ExtendedField9, NULL AS ExtendedField10
    UNION ALL  
    SELECT 'AW1', 'AW1', '4动2拖，Tc+Mp+M+M+Mp+Tc', NULL, NULL, '80km/h', '≥35km/h（平均站停时间30s）', '（1）列车从0加速到40km/h≥1.0m/s²；（2）列车从0加速到80km/h≥0.6m/s²', '（1）最大常用制动1.0m/s²；（2）紧急制动≥1.2m/s²；（3）快速制动≥1.2m/s²',   
    '（1）最大常用制动1.0m/s²；（2）紧急制动≥1.2m/s²；（3）快速制动≥1.2m/s²',  
    '≤0.75m/s³（紧急制动除外）',  
    '（1）一列6辆编组的列车在超载条件下，当损失1/4牵引动力时，列车仍然可以在用户现场最大坡道上起动，并能以正常运行方式完成当天运行；（2）一列6辆编组的列车在超载条件下，在损失1/2牵引动力时，列车仍然可以在用户现场最大坡道上起动和运行到最近车站的能力，清客后返回车辆段',  
    '（1）一列6辆编组的空载列车应能将另一列停在用户现场最大坡道上的6辆编组超载故障列车牵引至最近的车站（上坡）；（2）一列6辆编组的空载列车应能将另一列停在用户现场最大坡道上的6辆编组故障空载列车救援到车辆段（上坡）',  
    '（1）列车静止条件下辅助设备的噪音。列车处于静止状态和自由声场内，所有辅助设备正常运行时，客室内部沿车辆中心线、距离地板面1.5m高处至少测量3个点，测得的噪声水平≤69dB（A）。在空调回风口下方测得的噪声水平≤72dB（A）；（2）列车在地面线路道渣轨道上运行时的噪声。列车以正常方式加速、惰行或制动时，客室内部沿车辆中心线、距离地板面1.5m高处至少测量3个点，测得的噪声水平≤74dB（A），在贯通道附近和空调回风口下方，距离任意墙面不少于0.3m处，测得的噪声水平≤75dB（A）。司机室内噪声≤77dB（A）。列车以不超过80km/h的任意恒定速度（通常列车以60km/h±5%的恒定速度）运行时，车内中心距地板面高1.5m处测得的噪声水平≤73dB（A），司机室内测得的噪声≤74dB（A），恒速运行时间为60s',  
    '（1）静止条件下辅助设备的噪音。空载列车在静止状态和自由声场内，所有辅助设备同时运行时，沿水平方向距离走行轨线路中心线7.5m处，在列车任意一侧、列车长度范围内的任意点测得的噪音≤72dB（A）。测试在ISO3095规定的自由区域条件下，列车在露天地面区段进行。（2）列车在地面线路道渣轨道上运行时的噪声。列车以正常方式加速、惰行或减速运行时，沿水平方向距离线路中心线7.5m处测量，车辆发出的噪声≤80dB（A）。车外噪声的测试应根据ISO3095进行。列车以不超过80km/h（通常列车以60km/h±5%的恒定速度）运行时，沿水平方向距离线路中心线7.5m处测量，车辆发出的噪声≤80dB（A）',  
    NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL  
    UNION ALL  
    SELECT 'AW2', 'AW2', '4动2拖，Tc+Mp+M+M+Mp+Tc', NULL, NULL, '80km/h', '≥35km/h（平均站停时间30s）', '（1）列车从0加速到40km/h≥1.0m/s²；（2）列车从0加速到80km/h≥0.6m/s²', '（1）最大常用制动1.0m/s²；（2）紧急制动≥1.2m/s²；（3）快速制动≥1.2m/s²',   
    '（1）最大常用制动1.0m/s²；（2）紧急制动≥1.2m/s²；（3）快速制动≥1.2m/s²',  
    '≤0.75m/s³（紧急制动除外）',  
    '（1）一列6辆编组的列车在超载条件下，当损失1/4牵引动力时，列车仍然可以在用户现场最大坡道上起动，并能以正常运行方式完成当天运行；（2）一列6辆编组的列车在超载条件下，在损失1/2牵引动力时，列车仍然可以在用户现场最大坡道上起动和运行到最近车站的能力，清客后返回车辆段',  
    '（1）一列6辆编组的空载列车应能将另一列停在用户现场最大坡道上的6辆编组超载故障列车牵引至最近的车站（上坡）；（2）一列6辆编组的空载列车应能将另一列停在用户现场最大坡道上的6辆编组故障空载列车救援到车辆段（上坡）',  
    '（1）列车静止条件下辅助设备的噪音。列车处于静止状态和自由声场内，所有辅助设备正常运行时，客室内部沿车辆中心线、距离地板面1.5m高处至少测量3个点，测得的噪声水平≤69dB（A）。在空调回风口下方测得的噪声水平≤72dB（A）；（2）列车在地面线路道渣轨道上运行时的噪声。列车以正常方式加速、惰行或制动时，客室内部沿车辆中心线、距离地板面1.5m高处至少测量3个点，测得的噪声水平≤74dB（A），在贯通道附近和空调回风口下方，距离任意墙面不少于0.3m处，测得的噪声水平≤75dB（A）。司机室内噪声≤77dB（A）。列车以不超过80km/h的任意恒定速度（通常列车以60km/h±5%的恒定速度）运行时，车内中心距地板面高1.5m处测得的噪声水平≤73dB（A），司机室内测得的噪声≤74dB（A），恒速运行时间为60s',  
    '（1）静止条件下辅助设备的噪音。空载列车在静止状态和自由声场内，所有辅助设备同时运行时，沿水平方向距离走行轨线路中心线7.5m处，在列车任意一侧、列车长度范围内的任意点测得的噪音≤72dB（A）。测试在ISO3095规定的自由区域条件下，列车在露天地面区段进行。（2）列车在地面线路道渣轨道上运行时的噪声。列车以正常方式加速、惰行或减速运行时，沿水平方向距离线路中心线7.5m处测量，车辆发出的噪声≤80dB（A）。车外噪声的测试应根据ISO3095进行。列车以不超过80km/h（通常列车以60km/h±5%的恒定速度）运行时，沿水平方向距离线路中心线7.5m处测量，车辆发出的噪声≤80dB（A）',  
    NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL  
) AS tmp
WHERE NOT EXISTS (SELECT 1 FROM TestTrainInformation);

INSERT INTO protocolconfig (Category, CategoryRemark, Description, DescriptionRemark, Content,ContentRemark, Remark)
SELECT * FROM (
    SELECT '0x01' AS Category, '列车信息' AS CategoryRemark, '0x01' AS Description, '项目名称' AS DescriptionRemark, NULL AS Content, NULL AS ContentRemark, NULL AS Remark 
    UNION ALL SELECT '0x01', '列车信息', '0x02', '试验名称', '0x01', '制动系统性能试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x02', '试验名称', '0x02', '牵引系统性能试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x02', '试验名称', '0x03', '受流性能试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x02', '试验名称', '0x04', '动力学性能试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x02', '试验名称', '0x05', '噪声试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x03', '车型', '0x01', '动车', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x04', '试验对象', '0x01', '整车', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x05', '试验速度', '0x01', '0-80km/h', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x06', '载荷状态', '0x01', '空载AW0', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x06', '载荷状态', '0x02', '满载AW2', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x06', '载荷状态', '0x03', '超载AW3', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x07', '专业', '0x01', '制动系统性能试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x07', '专业', '0x02', '牵引系统性能试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x07', '专业', '0x03', '受流性能试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x07', '专业', '0x04', '动力学性能试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x07', '专业', '0x05', '噪声试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x08', '项点', '0x01', '静态传动效率试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x08', '项点', '0x02', '制动热容量试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x08', '项点', '0x03', '制动运行试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x08', '项点', '0x04', '起动加速试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x08', '项点', '0x05', '牵引特性试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x08', '项点', '0x06', '电制动试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x08', '项点', '0x07', '速度控制系统试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x08', '项点', '0x08', '运行阻力试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x08', '项点', '0x09', '网压中断试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x08', '项点', '0x0A', '受流性能试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x08', '项点', '0x0B', '动力学性能试验', NULL
    UNION ALL SELECT '0x01', '列车信息', '0x08', '项点', '0x0C', '噪声试验', NULL
    UNION ALL SELECT '0x02', '前置环境', '0x09', '供电', '0x01', '网压AC 25 kV～29 kV', NULL
    UNION ALL SELECT '0x02', '前置环境', '0x0A', '工务', '0x01', '平直道、坡道，线路黏着条件良好', NULL
    UNION ALL SELECT '0x02', '前置环境', '0x0A', '工务', '0x02', '坡道', NULL
    UNION ALL SELECT '0x02', '前置环境', '0x0A', '工务', '0x03', '直线、大曲线、小曲线、道岔直向、侧向、线路黏着条件良好', NULL
    UNION ALL SELECT '0x02', '前置环境', '0x0A', '工务', '0x04', '线路黏着条件良好', NULL
    UNION ALL SELECT '0x02', '前置环境', '0x0A', '工务', '0x05', '平直道，线路黏着条件良好', NULL
    UNION ALL SELECT '0x02', '前置环境', '0x0A', '工务', '0x06', '平直道，轨面相对地面0-2m,无雨雪，背景噪声低于被测噪声至少10dB以上', NULL
    UNION ALL SELECT '0x02', '前置环境', '0x0A', '工务', '0x07', '最小通过曲线，线路黏着条件良好', NULL
    UNION ALL SELECT '0x02', '前置环境', '0x0A', '工务', '0x08', '无', NULL
    UNION ALL SELECT '0x02', '前置环境', '0x0B', '环境', '0x01', '风速不大于3.3 m/s', NULL
    UNION ALL SELECT '0x02', '前置环境', '0x0B', '环境', '0x02', '风速不大于5m/s，空旷环境周围没有较大反射物', NULL
    UNION ALL SELECT '0x02', '前置环境', '0x0B', '环境', '0x03', '风速不大于5m/s', NULL
    UNION ALL SELECT '0x02', '前置环境', '0x0B', '环境', '0x04', '无', NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x0C', '列车速度', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x0D', '网压', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x0E', '网流', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x0F', '牵引制动级位', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x10', '牵引力需求值', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x11', '牵引力反馈值', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x12', '逆变器输出电压', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x13', '逆变器输出电流', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x14', '电机电流', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x15', '制动初速度', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x16', '实时加速度', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x17', '电制动力需求值', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x18', '电制动力反馈值', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x19', '制动距离', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x1A', '制动级位', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x1B', '制动指令', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x1C', '电制动切除', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x1D', '主断闭合断开', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x1E', '逆变器输入电流', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x1F', '逆变器温升', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x20', '电机温升', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x21', '站停时间', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x22', '列车总能耗', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x23', '牵引能耗', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x24', '再生能耗', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x25', '牵引级位', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x26', '加速度', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x27', '压力', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x28', '燃弧', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x29', '激光系统传感器', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x2A', '测速雷达', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x2B', '视频监控', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x2C', '拖车制动缸压力', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x2D', '动车制动缸压力', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x2E', '紧急制动指令', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x2F', '车辆紧急制动时的速度', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x30', '制动盘温度', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x31', '闸片温度', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x32', '拖车压力', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x33', '每站停站时间', NULL, NULL, NULL
    UNION ALL SELECT '0x03', '总控记录数据指令', '0x34', '对应车轮温度', NULL, NULL, NULL
    UNION ALL SELECT '0x04', '执行指令', '0x35', '制动缓解', '0x01', '制动缓解', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x36', '牵引级', '0x01', '0%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x36', '牵引级', '0x02', '10%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x36', '牵引级', '0x03', '20%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x36', '牵引级', '0x04', '30%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x36', '牵引级', '0x05', '40%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x36', '牵引级', '0x06', '50%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x36', '牵引级', '0x07', '60%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x36', '牵引级', '0x08', '70%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x36', '牵引级', '0x09', '80%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x36', '牵引级', '0x0A', '90%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x36', '牵引级', '0x0B', '100%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x37', '速度', NULL, NULL, NULL
    UNION ALL SELECT '0x04', '执行指令', '0x38', '惰行', '0x01', '惰行', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x39', '制动', '0x01', '0%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x39', '制动', '0x02', '10%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x39', '制动', '0x03', '20%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x39', '制动', '0x04', '30%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x39', '制动', '0x05', '40%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x39', '制动', '0x06', '50%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x39', '制动', '0x07', '60%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x39', '制动', '0x08', '70%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x39', '制动', '0x09', '80%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x39', '制动', '0x0A', '90%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x39', '制动', '0x0B', '100%', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x3A', '延时等待', NULL, '', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x3B', '网压中断', '0x01', '网压中断', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x3C', '网压恢复', '0x01', '网压恢复', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x3D', 'DCU', '0x01', 'DCU', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x3E', '连挂车辆载荷', '0x01', '连挂车辆载荷', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x3F', '电制动', '0x01', '电制动', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x41', '紧急制动', '0x01', '紧急制动', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x42', '保持制动', '0x01', '保持制动', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x43', 'For循环', NULL, NULL, NULL
    UNION ALL SELECT '0x04', '执行指令', '0x44', 'If判断', NULL, NULL, NULL
    UNION ALL SELECT '0x04', '执行指令', '0x45', 'M2主控', '0x01', 'M2主控', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x46', 'M1主控', '0x01', 'M1主控', NULL
    UNION ALL SELECT '0x04', '执行指令', '0x47', '', NULL, NULL, NULL
    UNION ALL SELECT '0x05', '返回值指令', '0x48', '开始获取数据', NULL, NULL, NULL
    UNION ALL SELECT '0x05', '返回值指令', '0x49', '结束获取数据', NULL, NULL, NULL
    UNION ALL SELECT '0x06', '返回分析评估指令', '0x4A', '开始获取数据', NULL, NULL, NULL
    UNION ALL SELECT '0x06', '返回分析评估指令', '0x4B', '结束获取数据', NULL, NULL, NULL
) AS tmp
WHERE NOT EXISTS (SELECT 1 FROM TestRecode);