import { MoveUp } from "lucide-react";
import React from "react";

const ScrollToTop = () => {
  return (
    <a href="#" className="fixed bottom-10 right-10">
      <div className="group bg-white h-12 w-12 shadow-xl text-base rounded-full flex items-center justify-center footer-icons hover:bg-ultramarine">
        <MoveUp className="group-hover:text-white transition-all duration-500" />
      </div>
    </a>
  );
};

export default ScrollToTop;
