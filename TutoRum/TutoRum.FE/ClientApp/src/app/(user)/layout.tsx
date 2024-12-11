"use client";
import React from "react";
import Navbar from "./components/Navbar/index";
import Footer from "./components/Footer/Footer";
import TestAiApi from "./components/TestAiApi";

const layout = ({ children }: { children: React.ReactNode }) => {
  return (
    <>
      <Navbar />
      <TestAiApi />
      {children}
      <Footer />
    </>
  );
};

export default layout;
