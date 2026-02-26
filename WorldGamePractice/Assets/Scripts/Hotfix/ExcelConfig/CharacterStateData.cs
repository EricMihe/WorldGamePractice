
using System.Collections.Generic;
using UnityEngine;

namespace Game.Config
{
    public class CharacterStateData
    {

        static CharacterStateData()
        {
            entityDic = new Dictionary<int, CharacterStateEntity>(52);
             CharacterStateEntity e0 = new CharacterStateEntity(1001,@"待机",@"idle",0,new float[]{1f,1f,1002f},0,new float[]{1f,1f,1019f},new float[]{1f,1f,1005f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{1f,1f,1009f},new float[]{1f,1f,1010f},new float[]{1f,1f,1011f},new float[]{1f,1f,1012f},new float[]{1f,1f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,1,1,5f,1,3f,0,0f,0f);
            entityDic.Add(e0.id, e0);
             CharacterStateEntity e1 = new CharacterStateEntity(1002,@"跑",@"run",0,null,10021,new float[]{1f,1f,1019f},new float[]{1f,1f,1005f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{1f,1f,1009f},new float[]{1f,1f,1010f},new float[]{1f,1f,1011f},new float[]{1f,1f,1012f},new float[]{1f,1f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,1,1,0f,1,0f,1,0f,0f);
            entityDic.Add(e1.id, e1);
             CharacterStateEntity e2 = new CharacterStateEntity(10021,@"跑衔接待机",@"run_end",1001,new float[]{1f,1f,1002f},0,new float[]{1f,1f,1019f},new float[]{1f,1f,1005f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{1f,1f,1009f},new float[]{1f,1f,1010f},new float[]{1f,1f,1011f},new float[]{1f,1f,1012f},new float[]{1f,1f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,1,1,0f,1,0f,0,0f,0f);
            entityDic.Add(e2.id, e2);
             CharacterStateEntity e3 = new CharacterStateEntity(1003,@"疾跑",@"sprint",0,null,10031,new float[]{1f,1f,1019f},new float[]{1f,1f,1005f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{1f,1f,1009f},new float[]{1f,1f,1010f},new float[]{1f,1f,1011f},new float[]{1f,1f,1012f},new float[]{1f,1f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,1,1,0f,1,0f,1,0f,0f);
            entityDic.Add(e3.id, e3);
             CharacterStateEntity e4 = new CharacterStateEntity(10031,@"疾跑衔接待机",@"sprint_end",1001,null,10031,new float[]{1f,1f,1019f},new float[]{1f,1f,1005f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{1f,1f,1009f},new float[]{1f,1f,1010f},new float[]{1f,1f,1011f},new float[]{1f,1f,1012f},new float[]{1f,1f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,1,1,0f,1,0f,1,0f,0f);
            entityDic.Add(e4.id, e4);
             CharacterStateEntity e5 = new CharacterStateEntity(1005,@"普攻1",@"attack_01",1001,null,0,new float[]{0f,0.75f,1006f},new float[]{0f,0.75f,1006f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,new int[]{1030,1030},0f,1,1,0f,1,0f,0,0.35f,0f);
            entityDic.Add(e5.id, e5);
             CharacterStateEntity e6 = new CharacterStateEntity(1006,@"普攻2",@"attack_02",1001,null,0,new float[]{0f,0.75f,1007f},new float[]{0f,0.75f,1007f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,new int[]{1030,1030},0f,1,1,0f,1,0f,0,0.35f,0f);
            entityDic.Add(e6.id, e6);
             CharacterStateEntity e7 = new CharacterStateEntity(1007,@"普攻3",@"attack_03",1001,null,0,new float[]{0f,0.75f,1008f},new float[]{0f,0.75f,1008f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,new int[]{1030,1030},0f,1,1,0f,1,0f,0,0.35f,0f);
            entityDic.Add(e7.id, e7);
             CharacterStateEntity e8 = new CharacterStateEntity(1008,@"普攻4",@"attack_04",1001,null,0,new float[]{0f,0.75f,1019f},new float[]{0f,0.75f,1005f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},null,0,new int[]{1017,1018},new int[]{1028,1029},0,new int[]{1030,1030},0f,1,1,0f,1,0f,0,0.35f,0f);
            entityDic.Add(e8.id, e8);
             CharacterStateEntity e9 = new CharacterStateEntity(1009,@"技能1",@"skill_01",1001,null,0,new float[]{0f,0.75f,1019f},new float[]{0f,0.75f,1005f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,new int[]{1030,1030},0f,0,0,0f,1,0f,0,0.35f,0f);
            entityDic.Add(e9.id, e9);
             CharacterStateEntity e10 = new CharacterStateEntity(1010,@"技能2",@"skill_02",1001,null,0,new float[]{0f,0.75f,1019f},new float[]{0f,0.75f,1005f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,new int[]{1030,1030},0f,0,0,0f,1,0f,0,0.35f,0f);
            entityDic.Add(e10.id, e10);
             CharacterStateEntity e11 = new CharacterStateEntity(1011,@"技能3",@"skill_03",1001,null,0,new float[]{0f,0.75f,1019f},new float[]{0f,0.75f,1005f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,new int[]{1030,1030},0f,0,0,0f,1,0f,0,0.35f,0f);
            entityDic.Add(e11.id, e11);
             CharacterStateEntity e12 = new CharacterStateEntity(1012,@"技能4",@"skill_04",1001,null,0,new float[]{0f,0.75f,1019f},new float[]{0f,0.75f,1005f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,new int[]{1030,1030},0f,0,0,0f,1,0f,0,0.35f,0f);
            entityDic.Add(e12.id, e12);
             CharacterStateEntity e13 = new CharacterStateEntity(1013,@"格挡起手",@"block_begin",10131,null,0,new float[]{0f,0.75f,1019f},new float[]{0f,0.75f,1005f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},1001,new float[]{1f,1f,1020f},0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},1031,null,0f,0,0,0f,0,0f,0,1f,0f);
            entityDic.Add(e13.id, e13);
             CharacterStateEntity e14 = new CharacterStateEntity(10131,@"格挡保持",@"block_keep",0,null,0,null,null,new float[]{1f,1f,1014f},null,10132,new float[]{1f,1f,1020f},0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},1031,null,0f,0,0,0f,0,0f,0,1f,0f);
            entityDic.Add(e14.id, e14);
             CharacterStateEntity e15 = new CharacterStateEntity(10132,@"格挡收",@"block_end",1001,null,0,null,null,new float[]{1f,1f,1014f},null,0,new float[]{1f,1f,1020f},0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,0,0,0f,1,0f,0,1f,0f);
            entityDic.Add(e15.id, e15);
             CharacterStateEntity e16 = new CharacterStateEntity(1014,@"突进",@"dash",1001,null,0,new float[]{0f,0.75f,1019f},new float[]{0f,0.75f,1005f},null,new float[]{1f,1f,1013f},0,null,0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,0,0,0f,0,0f,0,0.25f,0f);
            entityDic.Add(e16.id, e16);
             CharacterStateEntity e17 = new CharacterStateEntity(1015,@"前_受击",@"f_hit",1001,null,0,new float[]{0f,0.75f,1019f},new float[]{0f,0.75f,1005f},null,null,0,null,0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,0,0,0f,1,0f,0,0f,0f);
            entityDic.Add(e17.id, e17);
             CharacterStateEntity e18 = new CharacterStateEntity(1016,@"后_受击",@"b_hit",1001,null,0,null,null,null,null,0,null,0,null,null,null,null,null,new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,0,0,0f,1,0f,0,0f,0f);
            entityDic.Add(e18.id, e18);
             CharacterStateEntity e19 = new CharacterStateEntity(1017,@"前_击飞",@"f_bash",1026,null,0,null,null,null,null,0,null,0,null,null,null,null,null,null,0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,0,0,0f,1,0f,0,0f,0f);
            entityDic.Add(e19.id, e19);
             CharacterStateEntity e20 = new CharacterStateEntity(1018,@"后_击飞",@"b_bash",1027,null,0,null,null,new float[]{1f,1f,1014f},null,0,new float[]{1f,1f,1020f},0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},null,0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,0,0,0f,1,0f,0,0.5f,0f);
            entityDic.Add(e20.id, e20);
             CharacterStateEntity e21 = new CharacterStateEntity(1019,@"蓄力攻击",@"pow_attack",1001,null,0,null,new float[]{0.3f,1f,1005f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,new int[]{1030,1030},0f,0,0,0f,1,0f,0,0.35f,0f);
            entityDic.Add(e21.id, e21);
             CharacterStateEntity e22 = new CharacterStateEntity(1020,@"跳跃",@"jump_begin",1021,null,0,null,new float[]{1f,1f,1023f},null,null,0,new float[]{0f,0.1f,1020f},0,null,null,null,null,null,new int[]{1017,1018},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,0,0,0f,0,0f,0,1f,5f);
            entityDic.Add(e22.id, e22);
             CharacterStateEntity e23 = new CharacterStateEntity(1021,@"跳跃_下落",@"jump_loop",0,null,0,null,new float[]{1f,1f,1023f},null,null,0,new float[]{1f,1f,1020f},1022,null,null,null,null,null,new int[]{1017,1018},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,0,0,0f,0,0f,0,0f,3f);
            entityDic.Add(e23.id, e23);
             CharacterStateEntity e24 = new CharacterStateEntity(1022,@"跳跃_着地",@"jump_end",1001,null,0,null,new float[]{1f,1f,1023f},null,null,0,new float[]{1f,1f,1020f},0,null,null,null,null,null,new int[]{1017,1018},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,0,0,0f,0,0f,0,0.5f,0f);
            entityDic.Add(e24.id, e24);
             CharacterStateEntity e25 = new CharacterStateEntity(1023,@"跳跃攻击1",@"jump_atk01",1,null,0,null,new float[]{0f,0.75f,1024f},null,null,0,null,1001,null,null,null,null,null,new int[]{1017,1018},0,new int[]{1017,1018},new int[]{1028,1029},0,new int[]{1030,1030},0f,0,0,0f,0,0f,0,0.35f,0f);
            entityDic.Add(e25.id, e25);
             CharacterStateEntity e26 = new CharacterStateEntity(1024,@"跳跃攻击2",@"jump_atk02",1,null,0,null,new float[]{0f,0.75f,1025f},null,null,0,null,1001,null,null,null,null,null,new int[]{1017,1018},0,new int[]{1017,1018},new int[]{1028,1029},0,new int[]{1030,1030},0f,0,0,0f,0,0f,0,0.35f,0f);
            entityDic.Add(e26.id, e26);
             CharacterStateEntity e27 = new CharacterStateEntity(1025,@"跳跃攻击3",@"jump_atk03",1021,null,0,null,new float[]{0f,0.85f,1023f},null,null,0,null,1001,null,null,null,null,null,new int[]{1017,1018},0,new int[]{1017,1018},new int[]{1028,1029},0,new int[]{1030,1030},0f,0,0,0f,0,0f,0,0.35f,0f);
            entityDic.Add(e27.id, e27);
             CharacterStateEntity e28 = new CharacterStateEntity(1026,@"前_起身",@"f_getup",1001,null,0,null,null,null,null,0,null,0,null,null,null,null,null,new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,1,1,0f,0,0f,0,0f,0f);
            entityDic.Add(e28.id, e28);
             CharacterStateEntity e29 = new CharacterStateEntity(1027,@"后_起身",@"b_getup",1001,null,0,null,null,null,null,0,null,0,null,null,null,null,null,new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,1,1,0f,0,0f,0,0f,0f);
            entityDic.Add(e29.id, e29);
             CharacterStateEntity e30 = new CharacterStateEntity(1028,@"前_死亡",@"f_dead",0,null,0,null,null,null,null,0,null,0,null,null,null,null,null,null,0,null,null,0,null,0f,0,0,0f,0,0f,0,0f,0f);
            entityDic.Add(e30.id, e30);
             CharacterStateEntity e31 = new CharacterStateEntity(1029,@"后_死亡",@"b_dead",0,null,0,null,null,null,null,0,null,0,null,null,null,null,null,null,0,null,null,0,null,0f,0,0,0f,0,0f,0,0f,0f);
            entityDic.Add(e31.id, e31);
             CharacterStateEntity e32 = new CharacterStateEntity(1030,@"被格挡弹反",@"rebound",1001,null,0,null,null,null,new float[]{0.7f,0.7f,1013f},0,null,0,null,null,null,null,null,new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,0,0,0f,0,0f,0,0f,0f);
            entityDic.Add(e32.id, e32);
             CharacterStateEntity e33 = new CharacterStateEntity(1031,@"格挡成功",@"blocked",1001,new float[]{1f,1f,1002f},0,new float[]{1f,1f,1019f},new float[]{1f,1f,1005f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{1f,1f,1009f},new float[]{1f,1f,1010f},new float[]{1f,1f,1011f},new float[]{1f,1f,1012f},new float[]{1f,1f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0.2f,0,0,0f,0,0f,0,0f,0f);
            entityDic.Add(e33.id, e33);
             CharacterStateEntity e34 = new CharacterStateEntity(1032,@"前_躲闪",@"f_dodge",1001,null,0,null,null,null,null,0,null,0,null,null,null,null,null,new int[]{1015,1016},3,new int[]{1017,1018},new int[]{1028,1029},0,null,0.9f,1,1,0f,1,0f,0,0f,0f);
            entityDic.Add(e34.id, e34);
             CharacterStateEntity e35 = new CharacterStateEntity(1033,@"后_躲闪",@"b_dodge",1001,null,0,null,null,null,null,0,null,0,null,null,null,null,null,new int[]{1015,1016},3,new int[]{1017,1018},new int[]{1028,1029},0,null,0.9f,1,1,0f,1,0f,0,0f,0f);
            entityDic.Add(e35.id, e35);
             CharacterStateEntity e36 = new CharacterStateEntity(1034,@"左_躲闪",@"l_dodge",1001,null,0,null,null,null,null,0,null,0,null,null,null,null,null,new int[]{1015,1016},3,new int[]{1017,1018},new int[]{1028,1029},0,null,0.9f,1,1,0f,1,0f,0,0f,0f);
            entityDic.Add(e36.id, e36);
             CharacterStateEntity e37 = new CharacterStateEntity(1035,@"右_躲闪",@"r_dodge",1001,null,0,null,null,null,null,0,null,0,null,null,null,null,null,new int[]{1015,1016},3,new int[]{1017,1018},new int[]{1028,1029},0,null,0.9f,1,1,0f,1,0f,0,0f,0f);
            entityDic.Add(e37.id, e37);
             CharacterStateEntity e38 = new CharacterStateEntity(1036,@"前_踱步",@"f_pacing",0,null,0,null,null,null,null,0,null,0,null,null,null,null,null,new int[]{1015,1016},4,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,1,1,5f,0,0f,0,0f,0f);
            entityDic.Add(e38.id, e38);
             CharacterStateEntity e39 = new CharacterStateEntity(1037,@"后_踱步",@"b_pacing",0,null,0,null,null,null,null,0,null,0,null,null,null,null,null,new int[]{1015,1016},4,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,1,1,5f,0,0f,0,0f,0f);
            entityDic.Add(e39.id, e39);
             CharacterStateEntity e40 = new CharacterStateEntity(1038,@"左_踱步",@"l_pacing",0,null,0,null,null,null,null,0,null,0,null,null,null,null,null,new int[]{1015,1016},4,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,1,1,5f,0,0f,0,0f,0f);
            entityDic.Add(e40.id, e40);
             CharacterStateEntity e41 = new CharacterStateEntity(1039,@"右_踱步",@"r_pacing",0,null,0,null,null,null,null,0,null,0,null,null,null,null,null,new int[]{1015,1016},4,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,1,1,5f,0,0f,0,0f,0f);
            entityDic.Add(e41.id, e41);
             CharacterStateEntity e42 = new CharacterStateEntity(1040,@"左前_踱步",@"lf_pacing",0,null,0,null,null,null,null,0,null,0,null,null,null,null,null,new int[]{1015,1016},4,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,1,1,5f,0,0f,0,0f,0f);
            entityDic.Add(e42.id, e42);
             CharacterStateEntity e43 = new CharacterStateEntity(1041,@"右前_踱步",@"rf_pacing",0,null,0,null,null,null,null,0,null,0,null,null,null,null,null,new int[]{1015,1016},4,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,1,1,5f,0,0f,0,0f,0f);
            entityDic.Add(e43.id, e43);
             CharacterStateEntity e44 = new CharacterStateEntity(1042,@"移动到指定位置",@"run",0,null,0,new float[]{1f,1f,1019f},new float[]{1f,1f,1005f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{1f,1f,1009f},new float[]{1f,1f,1010f},new float[]{1f,1f,1011f},new float[]{1f,1f,1012f},new float[]{1f,1f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,1,1,5f,1,0f,0,0f,0f);
            entityDic.Add(e44.id, e44);
             CharacterStateEntity e45 = new CharacterStateEntity(1043,@"巡逻",@"walk",0,null,0,new float[]{1f,1f,1019f},new float[]{1f,1f,1005f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{1f,1f,1009f},new float[]{1f,1f,1010f},new float[]{1f,1f,1011f},new float[]{1f,1f,1012f},new float[]{1f,1f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,null,0f,1,1,5f,1,0f,0,0f,0f);
            entityDic.Add(e45.id, e45);
             CharacterStateEntity e46 = new CharacterStateEntity(1044,@"处决",@"execut",1001,null,0,null,null,null,null,0,null,0,null,null,null,null,null,null,0,null,null,0,null,0f,0,0,0f,0,0f,0,0f,0f);
            entityDic.Add(e46.id, e46);
             CharacterStateEntity e47 = new CharacterStateEntity(1045,@"被处决",@"be_execut",-1,null,0,null,null,null,null,0,null,0,null,null,null,null,null,null,0,null,null,0,null,0f,0,0,0f,0,0f,0,0f,0f);
            entityDic.Add(e47.id, e47);
             CharacterStateEntity e48 = new CharacterStateEntity(1046,@"使用暗器_炸弹",@"use_prop",1001,null,0,new float[]{0f,0.75f,1006f},new float[]{0f,0.75f,1006f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,new int[]{1030,1030},0f,1,1,0f,1,0f,0,0.35f,0f);
            entityDic.Add(e48.id, e48);
             CharacterStateEntity e49 = new CharacterStateEntity(1047,@"使用暗器_飞镖",@"use_prop",1001,null,0,new float[]{0f,0.75f,1006f},new float[]{0f,0.75f,1006f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,new int[]{1030,1030},0f,1,1,0f,1,0f,0,0.35f,0f);
            entityDic.Add(e49.id, e49);
             CharacterStateEntity e50 = new CharacterStateEntity(1048,@"使用暗器_飞刀",@"use_prop",1001,null,0,new float[]{0f,0.75f,1006f},new float[]{0f,0.75f,1006f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,new int[]{1030,1030},0f,1,1,0f,1,0f,0,0.35f,0f);
            entityDic.Add(e50.id, e50);
             CharacterStateEntity e51 = new CharacterStateEntity(1049,@"使用暗器_圆轮",@"use_prop",1001,null,0,new float[]{0f,0.75f,1006f},new float[]{0f,0.75f,1006f},new float[]{1f,1f,1014f},new float[]{1f,1f,1013f},0,new float[]{1f,1f,1020f},0,new float[]{0f,0.75f,1009f},new float[]{0f,0.75f,1010f},new float[]{0f,0.75f,1011f},new float[]{0f,0.75f,1012f},new float[]{0f,0.75f,1046f},new int[]{1015,1016},0,new int[]{1017,1018},new int[]{1028,1029},0,new int[]{1030,1030},0f,1,1,0f,1,0f,0,0.35f,0f);
            entityDic.Add(e51.id, e51);

        }

       
        
        public static Dictionary<int, CharacterStateEntity> all {
            get {
                return entityDic;
            }
        }
		static Dictionary<int, CharacterStateEntity> entityDic;
		public static CharacterStateEntity Get(int id)
		{
            if (entityDic!=null&&entityDic.TryGetValue(id,out var entity))
			{
				return entity;
			}
            return null;
		}
    }

    
    public class CharacterStateEntity
    {
        //TemplateMember
		public int id;//状态ID
		public string info;//说明
		public string anm_name;//动画名称
		public int on_anm_end;//当动画播放结束
		public float[] on_move;//当进行移动
		public int on_stop;//当停止移动
		public float[] on_pow_atk;//当进行蓄力攻击
		public float[] on_atk;//当进行普攻
		public float[] on_sprint;//当进行冲刺
		public float[] on_defense;//当进行格挡
		public int on_defense_quit;//当取消格挡
		public float[] on_jump;//当进行跳跃
		public int on_jump_end;//当跳跃接地
		public float[] on_skill1;//当使用技能1
		public float[] on_skill2;//当使用技能2
		public float[] on_skill3;//当使用技能3
		public float[] on_skill4;//当使用技能4
		public float[] on_use_prop;//当使用暗器
		public int[] on_hit;//当普通受击
		public int tag;//标签
		public int[] on_bash;//当受到击飞
		public int[] on_death;//当死亡时
		public int on_block_succes;//当成功格挡
		public int[] be_block;//当攻击被格挡
		public float trigger_atk;//调度攻击决策
		public int trigger_dodge;//触发躲闪
		public int first_strike;//触发抢攻
		public float active_attack;//随机发起攻击
		public int trigger_pacing;//AI进入踱步状态
		public float trigger_patrol;//执行巡逻
		public int do_move;//执行移动
		public float do_rotate;//朝向控制
		public float add_f_move;//叠加正向位移

        public CharacterStateEntity(){}
        public CharacterStateEntity(int id,string info,string anm_name,int on_anm_end,float[] on_move,int on_stop,float[] on_pow_atk,float[] on_atk,float[] on_sprint,float[] on_defense,int on_defense_quit,float[] on_jump,int on_jump_end,float[] on_skill1,float[] on_skill2,float[] on_skill3,float[] on_skill4,float[] on_use_prop,int[] on_hit,int tag,int[] on_bash,int[] on_death,int on_block_succes,int[] be_block,float trigger_atk,int trigger_dodge,int first_strike,float active_attack,int trigger_pacing,float trigger_patrol,int do_move,float do_rotate,float add_f_move){
           
           this.id = id;
           this.info = info;
           this.anm_name = anm_name;
           this.on_anm_end = on_anm_end;
           this.on_move = on_move;
           this.on_stop = on_stop;
           this.on_pow_atk = on_pow_atk;
           this.on_atk = on_atk;
           this.on_sprint = on_sprint;
           this.on_defense = on_defense;
           this.on_defense_quit = on_defense_quit;
           this.on_jump = on_jump;
           this.on_jump_end = on_jump_end;
           this.on_skill1 = on_skill1;
           this.on_skill2 = on_skill2;
           this.on_skill3 = on_skill3;
           this.on_skill4 = on_skill4;
           this.on_use_prop = on_use_prop;
           this.on_hit = on_hit;
           this.tag = tag;
           this.on_bash = on_bash;
           this.on_death = on_death;
           this.on_block_succes = on_block_succes;
           this.be_block = be_block;
           this.trigger_atk = trigger_atk;
           this.trigger_dodge = trigger_dodge;
           this.first_strike = first_strike;
           this.active_attack = active_attack;
           this.trigger_pacing = trigger_pacing;
           this.trigger_patrol = trigger_patrol;
           this.do_move = do_move;
           this.do_rotate = do_rotate;
           this.add_f_move = add_f_move;

        }
    }
}
