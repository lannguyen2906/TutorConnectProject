import { cookies } from "next/headers";
import { NextResponse } from "next/server";

// Named export for the POST request handler
// Named export for the POST request handler (Login)
export async function POST(request: Request) {
  try {
    const body = await request.json();
    const { sessionToken } = body;
    const cookieStore = cookies();

    if (!sessionToken) {
      return NextResponse.json(
        { message: "Session token is required" },
        { status: 400 }
      );
    }

    // Set cookie with sessionToken
    const response = NextResponse.json(
      { message: "Login successful" },
      { status: 200 }
    );

    cookieStore.set("sessionToken", sessionToken);

    return response;
  } catch (error: any) {
    return NextResponse.json(
      { message: "Internal Server Error", error: error.message },
      { status: 500 }
    );
  }
}
