#include <Windows.h>

const int KeyLen = 4;

/// <summary>
/// CheckCDKey
/// </summary>
/// <param name="machineCodeMd5">用户码的MD5值，长度一定是int[4]</param>
/// <param name="cdk">程序授权码，长度一定是int[4]</param>
/// <return>是否通过的标识, 0通过，other错误的cdk<return>
extern "C" __declspec(dllexport) int CheckCDKey(const unsigned int* usrCodeMd5, const unsigned int * cdk)
{
	const unsigned int uuid[] = {0xBADC1234, 0x12345678, 0x146790EF, 0xABCD4321};
	for (int index = 0; index != KeyLen; ++index)
	{
		const unsigned int lVal = usrCodeMd5[index] ^ cdk[index];
		if (uuid[index] != lVal)
		{
			return 1;
		}
	}
	return 0;
}

extern "C" __declspec(dllexport) int CheckCDKeyI(
	const int usrCodeMd50, 
	const int usrCodeMd51,
	const int usrCodeMd52,
	const int usrCodeMd53,
	const int cdk0,
	const int cdk1,
	const int cdk2,
	const int cdk3)
{

	return 0;
}