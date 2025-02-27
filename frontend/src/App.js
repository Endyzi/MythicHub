import { useState } from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import { AppBar, Toolbar, Typography, Container, TextField, Button, Box, MenuItem } from "@mui/material";
import Navbar from "./components/Navbar";
import CharacterProfile from "./components/CharacterProfile";
import Home from "./components/Home";
import "@fontsource/cinzel-decorative"; // font
import React from "react";


 export default function App() {
  const [searchQuery, setSearchQuery] = useState("");

  const handleSearch = () => {
    console.log("Searching for:", searchQuery);
  };

  return (
  <Router>
      <Box sx={{ backgroundColor: "#0b0f19", minHeight: "100vh", color: "#ffffff" }}>
        <Navbar searchQuery={searchQuery} setSearchQuery={setSearchQuery} handleSearch={handleSearch} />
        <Container sx={{ textAlign: "center", mt: 4 }}>
          <Routes>
            {/* Homepage with top 20 players */}
            <Route
              path="/"
              element={
                <>
                  <Typography variant="h4" sx={{ mb: 2 }}>
                    Top 20 Players
                  </Typography>
                  <Box sx={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(200px, 1fr))", gap: 3 }}>
                    {[...Array(20)].map((_, index) => (
                      <Box key={index} sx={{ width: 200, height: 250, backgroundColor: "#1a0236", borderRadius: 2, p: 2 }}>
                        <Typography>Player {index + 1}</Typography>
                      </Box>
                    ))}
                  </Box>
                </>
              }
            />
            {/* Profile page for a character */}
            <Route path="/character/:region/:realm/:name" element={<CharacterProfile />} />
          </Routes>
        </Container>
      </Box>
    </Router>
  );
}
