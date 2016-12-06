namespace Garply
{
    public enum Types : uint
    {
        Error      = ( 0x00000000                      )             ,
        Abstract   = ( 0x00000001                      )             ,
        Value      = ( 0x00000002                      ) | Abstract  ,
        Reference  = ( 0x00000004                      ) | Abstract  ,
        Number     = ( 0x00000008                      ) | Abstract  ,
        Tuple      = ( 0x00000010 | Reference          ) & ~Abstract ,
        List       = ( 0x00000020 | Reference          ) & ~Abstract ,
        String     = ( 0x00000040 | Reference          ) & ~Abstract ,
        Expression = ( 0x00000080 | Reference          ) & ~Abstract ,
        Type       = ( 0x00000100 | Value              ) & ~Abstract ,
        Boolean    = ( 0x00000200 | Value              ) & ~Abstract ,
        Integer    = ( 0x00000400 | Value     | Number ) & ~Abstract ,
        Float      = ( 0x00000800 | Value     | Number ) & ~Abstract ,
    }
}
