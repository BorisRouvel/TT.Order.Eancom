using System;
using System.Collections.Generic;

using Ord_Eancom;

namespace Eancom
{
    public class FTX_H
    {
        OrderInformations _orderInformations = null;
        C108 c108 = null;

        public const string E4451_ZZZ = "ZZZ";
        public const string E4451_AAI = "AAI";

        private readonly string E3453 = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToUpper();

        private string _e4451;
        public string E4451
        {
            get
            {
                return _e4451;
            }
            set
            {
                _e4451 = value;
            }
        }

        public class C108
        {
            private string _e4440;
            public string E4440
            {
                get
                {
                    return _e4440;
                }
                set
                {
                    _e4440 = value;
                }
            }

            public C108(string e4440)
            {
                _e4440 = e4440;
            }

            public string Add() 
            {               
                return this.E4440;
            }
        }

      
        public FTX_H(OrderInformations orderInformations)
        {
            _orderInformations = orderInformations;                     
        }

        public string Add()
        {
            string fileType = String.Empty;
            string fileFormat =  String.Empty;
            string fileName =  String.Empty;
            string fileDescription = String.Empty;
            string fileInformation = String.Empty;

            if (MainForm.IsChoiceExportEGI)
            {
                fileType = Convert.ToInt16(OrderEnum.Type.EDIGRAPH).ToString();
                fileFormat = Convert.ToInt16(OrderEnum.Format.EDIGRAPH).ToString();
                fileName = OrderTransmission.OrderEGIFileName;
                fileDescription = OrderTransmission.OrderEGIFileName;

                fileInformation += this.SetAdd(fileType + KD.StringTools.Const.SemiColon + fileFormat + KD.StringTools.Const.SemiColon + fileName + KD.StringTools.Const.SemiColon + fileDescription);
            }

            if (MainForm.IsChoiceExportPlan)
            {
                int index = 1;

                fileType = Convert.ToInt16(OrderEnum.Type.FLOOR_PLAN).ToString();
                fileFormat = Convert.ToInt16(OrderEnum.Format.JPEG).ToString();
                
                string[] allJPGFiles = System.IO.Directory.GetFiles(Order.orderDir, KD.StringTools.Const.Wildcard + OrderTransmission.ExtensionJPG);
                foreach (string jPGfile in allJPGFiles)
                {
                    if (jPGfile.ToUpper().Contains(OrderTransmission.PlanName.ToUpper()))
                    {
                        fileName = System.IO.Path.GetFileName(jPGfile);//  OrderTransmission.PlanName + index + OrderTransmission.ExtensionJPG;
                        fileDescription = System.IO.Path.GetFileName(jPGfile);// OrderTransmission.PlanName + index + OrderTransmission.ExtensionJPG;

                        fileInformation += this.SetAdd(fileType + KD.StringTools.Const.SemiColon + fileFormat + KD.StringTools.Const.SemiColon + fileName + KD.StringTools.Const.SemiColon + fileDescription);

                        index++;
                    }
                }
                
            }

            if (MainForm.IsChoiceExportElevation)
            {
                int index = 1;

                fileType = Convert.ToInt16(OrderEnum.Type.WALL_FRONT_VIEW).ToString();
                fileFormat = Convert.ToInt16(OrderEnum.Format.JPEG).ToString();

                string[] allJPGFiles = System.IO.Directory.GetFiles(Order.orderDir, KD.StringTools.Const.Wildcard + OrderTransmission.ExtensionJPG);
                foreach (string jPGfile in allJPGFiles)
                {
                    if (jPGfile.ToUpper().Contains(OrderTransmission.ElevName.ToUpper()))
                    {
                        fileName = System.IO.Path.GetFileName(jPGfile);// OrderTransmission.ElevName + index + OrderTransmission.ExtensionJPG;
                        fileDescription = System.IO.Path.GetFileName(jPGfile);// OrderTransmission.ElevName + index + OrderTransmission.ExtensionJPG;

                        fileInformation += this.SetAdd(fileType + KD.StringTools.Const.SemiColon + fileFormat + KD.StringTools.Const.SemiColon + fileName + KD.StringTools.Const.SemiColon + fileDescription);
                        index++;
                    }
                }
            }

            if (MainForm.IsChoiceExportPerspective)
            {
                int index = 1;

                fileType = Convert.ToInt16(OrderEnum.Type.PERSPECTIVE).ToString();
                fileFormat = Convert.ToInt16(OrderEnum.Format.JPEG).ToString();

                string[] allJPGFiles = System.IO.Directory.GetFiles(Order.orderDir, KD.StringTools.Const.Wildcard + OrderTransmission.ExtensionJPG);
                foreach (string jPGfile in allJPGFiles)
                {
                    if (jPGfile.ToUpper().Contains(OrderTransmission.PerspectiveName.ToUpper()))
                    {
                        fileName = System.IO.Path.GetFileName(jPGfile);
                        fileDescription = System.IO.Path.GetFileName(jPGfile);

                        fileInformation += this.SetAdd(fileType + KD.StringTools.Const.SemiColon + fileFormat + KD.StringTools.Const.SemiColon + fileName + KD.StringTools.Const.SemiColon + fileDescription);
                        index++;
                    }
                }
            }

            if (MainForm.IsChoiceExportOrder)
            {
                fileType = Convert.ToInt16(OrderEnum.Type.OTHER).ToString();
                fileFormat = Convert.ToInt16(OrderEnum.Format.PDF).ToString();
                fileName = OrderTransmission.OrderName + OrderTransmission.ExtensionPDF;
                fileDescription = OrderTransmission.OrderName + OrderTransmission.ExtensionPDF;

                fileInformation += this.SetAdd(fileType + KD.StringTools.Const.SemiColon + fileFormat + KD.StringTools.Const.SemiColon + fileName + KD.StringTools.Const.SemiColon + fileDescription);
            }

            if (!String.IsNullOrEmpty(fileInformation))
            {               
                return fileInformation;
            }
            return null;
        }
        private string SetAdd(string information)
        {
            c108 = new C108(information);
            _e4451 = Eancom.FTX_H.E4451_ZZZ + Separator.DataGroup + Separator.DataGroup + Separator.DataGroup;

            OrderWrite.segmentNumberBetweenUNHandUNT += 1;
            return StructureEDI.FTX_H + Separator.DataGroup + this.E4451 + c108.Add() + Separator.DataGroup + this.E3453 + Separator.EndLine;
        }

        public List<string> AddComment()
        {
            List<string> FTXCommentList = new List<string>();            
            _e4451 = Eancom.FTX_H.E4451_AAI + Separator.DataGroup + Separator.DataGroup + Separator.DataGroup;

            string commentScene = _orderInformations.GetCommentScene();
            if (!String.IsNullOrEmpty(commentScene))
            {                
                string truncatedCommentScene = string.Empty;
                commentScene = commentScene.Replace("\r\n", String.Empty);
                int start = 0;
                for (int line = 0; line < OrderConstants.CommentSceneLinesMax; line++)
                {
                    if (OrderConstants.CommentSceneCharactersPerLineMax + start > commentScene.Length)
                    {
                        int len = (commentScene.Length - (OrderConstants.CommentSceneCharactersPerLineMax + start));
                        truncatedCommentScene = commentScene.Substring(start, (OrderConstants.CommentSceneCharactersPerLineMax + len));
                        c108 = new C108(truncatedCommentScene);

                        OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                        FTXCommentList.Add(StructureEDI.FTX_H + Separator.DataGroup + this.E4451 + c108.Add() + Separator.DataGroup + this.E3453 + Separator.EndLine); 

                        break;
                    }
                    truncatedCommentScene = commentScene.Substring(start, OrderConstants.CommentSceneCharactersPerLineMax);
                    c108 = new C108(truncatedCommentScene);

                    OrderWrite.segmentNumberBetweenUNHandUNT += 1;
                    FTXCommentList.Add(StructureEDI.FTX_H + Separator.DataGroup + this.E4451 + c108.Add() + Separator.DataGroup + this.E3453 + Separator.EndLine);
                    start += OrderConstants.CommentSceneCharactersPerLineMax;
                }
            }
            return FTXCommentList;
        }
    }
}
