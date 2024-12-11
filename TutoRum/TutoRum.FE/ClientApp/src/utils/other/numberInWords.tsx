var writtenNumber = require("written-number");

export const numberInWords = (num: string) => {
  const numValue = parseInt(num, 10);
  return writtenNumber(numValue, { lang: "vi" }) + " đồng";
};
