using Campus.DocumentValidator;

namespace BodyTemperature
{
    /// <summary>
    /// 用來產生排課系統所需的自訂驗證規則
    /// </summary>
    public class BodyTmperatureFactory : IFieldValidatorFactory
    {
        #region IFieldValidatorFactory 成員

        /// <summary>
        /// 根據typeName建立對應的FieldValidator
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="validatorDescription"></param>
        /// <returns></returns>
        public IFieldValidator CreateFieldValidator(string typeName, System.Xml.XmlElement validatorDescription)
        {
            switch (typeName.ToUpper())
            {
                case "STUDENTNUMBEREXISTENCEBODY":
                    return new BodyTmperatureFactoryExistenceValidator(); //
                case "STUDENTNUMBERREPEATBODY":
                    return new BodyTmperatureFactoryRepeatValidator(); //
                case "STUDENTNUMBERSTATUSBODY":
                    return new BodyTmperatureFactoryStatusValidator(); //學生狀況是一般或延修生
                default:
                    return null;
            }
        }

        #endregion
    }
}