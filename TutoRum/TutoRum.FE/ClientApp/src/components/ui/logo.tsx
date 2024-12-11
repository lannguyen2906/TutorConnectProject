import Image from "next/image";
import React from "react";

const Logo = ({ hasText, size = 300 }: { hasText: boolean; size?: number }) => {
  return (
    <div className="flex flex-shrink-0 items-center gap-3">
      {hasText ? (
        <Image
          width={size}
          height={size}
          priority
          src={"/assets/logo/logotext.svg"}
          alt="dsign-logo"
        />
      ) : (
        <Image
          width={size}
          height={size}
          priority
          src={"/assets/logo/logonotext.svg"}
          alt="dsign-logo"
        />
      )}
    </div>
  );
};

export default Logo;
