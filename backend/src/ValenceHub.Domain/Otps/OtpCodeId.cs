using ValenceHub.Domain.Common.Primitives;

namespace ValenceHub.Domain.Otps;

public sealed class OtpCodeId : StronglyTypedId<Guid>
{
    private OtpCodeId(Guid value)
        : base(value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("OtpCodeId cannot be empty.", nameof(value));
    }

    public static OtpCodeId New() => new(Guid.NewGuid());

    public static OtpCodeId FromGuid(Guid value) => new(value);

    public static implicit operator Guid(OtpCodeId otpCodeId) => otpCodeId.Value;
}
