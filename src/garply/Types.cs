namespace Garply
{
    internal enum Types : uint
    {
        error      = ( 0x00000000                      )             ,
        Abstract   = ( 0x00000001                      )             ,
        Value      = ( 0x00000002                      ) | Abstract  ,
        Reference  = ( 0x00000004                      ) | Abstract  ,
        Number     = ( 0x00000008                      ) | Abstract  ,
        tuple      = ( 0x00000010 | Reference          ) & ~Abstract ,
        list       = ( 0x00000020 | Reference          ) & ~Abstract ,
        @string    = ( 0x00000040 | Reference          ) & ~Abstract ,
        expression = ( 0x00000080 | Reference          ) & ~Abstract ,
        type       = ( 0x00000100 | Value              ) & ~Abstract ,
        @bool      = ( 0x00000200 | Value              ) & ~Abstract ,
        @int       = ( 0x00000400 | Value     | Number ) & ~Abstract ,
        @float     = ( 0x00000800 | Value     | Number ) & ~Abstract ,

        Any        = 0xFFFFFFFF
    }
}
