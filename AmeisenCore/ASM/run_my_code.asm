﻿PUSHFD
PUSHAD

MOV EBX, [{0}]
TEST EBX, 1
JE @out

MOV EDX, {1}
CALL EDX");
MOV [{2}], EAX

@out:
MOV EDX, 0
MOV [{3}], EDX

POPAD
POPFD